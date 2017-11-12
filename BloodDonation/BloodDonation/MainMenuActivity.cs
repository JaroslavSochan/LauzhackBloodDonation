using AForge.Neuro;
using AForge.Neuro.Learning;
using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Android;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;


namespace BloodDonation
{
    [Activity(Label = "MainMenuActivity", Theme = "@android:style/Theme.Holo.Light")]
    public class MainMenuActivity : Activity
    {
        private TextView textViewLastBloodPicture;
        private TextView textViewAnalyzeName;
        private TextView textViewAnalyzeResults;
        private TextView textViewShouldDo;
        private TextView textViewShouldDo1;
        private TextView textViewShouldDo2;

        private ImageView imageViewShuldDo1;
        private ImageView imageViewShuldDo2;

        private Button buttonGetLastBP;
        private Button buttonAnalyzeBP;
        private Button buttonGetPlan;

        private ProgressDialog progressDialog;

        private BloodDonatonNeuralNet neuralNetwork;

        private BloodDonatonNeuralNet.BloodState BloodState;

        private PlotView BloodPictureGraph;
        private bool needToStop = false;
        private const int dataSetArrayCount = 500;
        private const int dataSetCount = 100;
        private static string filename = "PretrainedNeuralNetwork.xml";

        private double[] bloodPictureData;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainMenu);
            // Create your application here

            textViewLastBloodPicture = FindViewById<TextView>(Resource.Id.textViewLastBloodPicture);
            textViewAnalyzeName = FindViewById<TextView>(Resource.Id.textViewAnalyzeName);
            textViewAnalyzeResults = FindViewById<TextView>(Resource.Id.textViewAnalyzeResults);
            textViewShouldDo = FindViewById<TextView>(Resource.Id.textViewShouldDo);

            textViewShouldDo1 = FindViewById<TextView>(Resource.Id.textViewShouldDo1);
            textViewShouldDo2 = FindViewById<TextView>(Resource.Id.textViewShouldDo2);

            imageViewShuldDo1 = FindViewById<ImageView>(Resource.Id.imageViewShuldDo1);
            imageViewShuldDo2 = FindViewById<ImageView>(Resource.Id.imageViewShuldDo2);

            buttonGetLastBP = FindViewById<Button>(Resource.Id.buttonGetLastBP);
            buttonGetLastBP.Click += ButtonGetLastBP_Click;
            buttonAnalyzeBP = FindViewById<Button>(Resource.Id.buttonAnalyzeBP);
            buttonAnalyzeBP.Click += ButtonAnalyzeBP_Click;
            buttonGetPlan = FindViewById<Button>(Resource.Id.buttonGetPlan);
            buttonGetPlan.Click += ButtonGetPlan_Click;

            BloodPictureGraph = FindViewById<PlotView>(Resource.Id.BloodPictureGraph);

            buttonGetPlan.Visibility = ViewStates.Invisible;
            textViewAnalyzeName.Visibility = ViewStates.Invisible;
            textViewAnalyzeResults.Visibility = ViewStates.Invisible;
            textViewShouldDo.Visibility = ViewStates.Invisible;

            progressDialog = new ProgressDialog(this);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.SetMessage("Calculating...");
            progressDialog.SetCancelable(false);
            progressDialog.Create();

            progressDialog.Show();

            //await TrainNeuralNetwork();
            neuralNetwork = await LoadNeuralNetwork();

            bloodPictureData = DatabaseAdapter.GetLastBloodData().Result;

            BloodPictureGraph.Model = CalculateBP();
            progressDialog.Hide();
        }

        public async Task TrainNeuralNetwork()
        {
            BloodDonatonNeuralNet net = new BloodDonatonNeuralNet();
            await Task.Run(() =>
            {
                try
                {
                    Init();
                }
                catch (Exception ex)
                {

                }
            });
        }

        public async Task<BloodDonatonNeuralNet> LoadNeuralNetwork()
        {
            BloodDonatonNeuralNet net = new BloodDonatonNeuralNet();
            await Task.Run(() =>
            {
                try
                {
                    var FullFilePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
                    BinaryFormatter bf = new BinaryFormatter();
                    using (Stream fsout = new FileStream(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        bf.Binder = new PreMergeToMergedDeserializationBinder();
                        net = (BloodDonatonNeuralNet)bf.Deserialize(fsout);
                    }
                }
                catch (Exception ex)
                {

                }
            });

            return net;
        }

        public void Init()
        {
            var dataset = CreateDataSet();
            int counter = 0;

            needToStop = false;
            // initialize input and output values
            double[][] input = new double[dataSetArrayCount][];
            double[][] output = new double[dataSetArrayCount][];

            for (int i = 0; i < dataset.Count; i++)
            {
                input[i] = new double[6];
                input[i][0] = dataset[i].Erytrocyt;
                input[i][1] = dataset[i].Fibrinogen;
                input[i][2] = dataset[i].Hemocyt;
                input[i][3] = dataset[i].Leukocyt;
                input[i][4] = dataset[i].Protrombin;
                input[i][5] = dataset[i].Trombocyt;

                output[i] = new double[2];
                output[i][0] = dataset[i].Result1;
                output[i][1] = dataset[i].Result2;
            }



            SigmoidFunction sigmoidFunction = new SigmoidFunction(5);
            // create neural network
            ActivationNetwork network = new ActivationNetwork(
                sigmoidFunction,
                6, // two inputs in the network
                5, // two neurons in the first layer
                2); // one neuron in the second layer
                    // create teacher

            network.Randomize();
            BackPropagationLearning teacher =
                new BackPropagationLearning(network);
            teacher.LearningRate = 5;
            // loop
            while (!needToStop)
            {
                // run epoch of learning procedure
                double error = teacher.RunEpoch(input, output);
                // check error value to see if we need to stop
                // ...

                counter++;
                if (counter == 2000)
                {
                    Console.WriteLine(error);
                    counter = 0;
                }
                //if (error < 200)
                if (error < 0.05)
                    needToStop = true;
            }

            BloodDonatonNeuralNet bloodDonatonNeuralNet = new BloodDonatonNeuralNet();
            bloodDonatonNeuralNet.NeuralNetwork = network;

            AssetManager assets = this.Assets;
            IFormatter formatter;
            using (MemoryStream stream = new MemoryStream())
            {
                formatter = new BinaryFormatter();
                formatter.Serialize(stream, bloodDonatonNeuralNet);

                var FullFilePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);

                stream.Seek(0, SeekOrigin.Begin);

                using (FileStream fs = new FileStream(FullFilePath, FileMode.OpenOrCreate))
                {
                    stream.CopyTo(fs);
                    fs.Flush();
                }
            }
        }

        public List<BloodPictureItem> CreateDataSet()
        {
            Random random = new Random();

            ConcurrentBag<BloodPictureItem> dataSetList = new ConcurrentBag<BloodPictureItem>();

            Parallel.For(0, dataSetCount,
                   index =>
                   {

                       BloodPictureItem item = new BloodPictureItem();

                       //// A
                       item.Erytrocyt = ((double)random.Next(500, 1000)) / 1000.0;
                       item.Fibrinogen = ((double)random.Next(500, 800)) / 1000.0;

                       item.Result1 = (double)Results.Hurt / 10;
                       item.Result2 = (double)Results.Kidney / 10;

                       dataSetList.Add(item);
                       //// B
                       item = new BloodPictureItem();
                       item.Erytrocyt = ((double)random.Next(500, 700)) / 1000.0;
                       item.Trombocyt = ((double)random.Next(500, 700)) / 1000.0;

                       item.Result1 = (double)Results.Hurt / 10;
                       item.Result2 = (double)Results.Hydrated / 10;
                       dataSetList.Add(item);

                       //// C
                       item = new BloodPictureItem();
                       item.Leukocyt = ((double)random.Next(500, 1000)) / 1000.0;
                       item.Protrombin = ((double)random.Next(500, 700)) / 1000.0;

                       item.Result1 = (double)Results.Oxygenation / 10;
                       item.Result2 = (double)Results.Kidney / 10;
                       dataSetList.Add(item);

                       //// D
                       item = new BloodPictureItem();
                       item.Hemocyt = ((double)random.Next(500, 1000)) / 1000.0;
                       item.Erytrocyt = ((double)random.Next(500, 1000)) / 1000.0;

                       item.Result1 = (double)Results.Sick / 10;
                       item.Result2 = (double)Results.Hurt / 10;
                       dataSetList.Add(item);

                       //// E
                       item = new BloodPictureItem();
                       item.Trombocyt = ((double)random.Next(500, 700)) / 1000.0;
                       item.Leukocyt = ((double)random.Next(500, 700)) / 1000.0;

                       item.Result1 = (double)Results.Hydrated / 10;
                       item.Result2 = (double)Results.Oxygenation / 10;
                       //// F
                       dataSetList.Add(item);
                   });

            return new List<BloodPictureItem>(dataSetList);
        }

        private PlotModel CalculateBP()
        {

            PlotModel plotModel = new PlotModel { Title = "Blood picture" };
            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                Key = "CakeAxis",
                ItemsSource = new[]
                {
                "Erytrocyt",
                "Fibrinogen",
                "Hemocyt",
                "Leukocyt",
                "Protrombin",
                "Trombocyt"
                }
            });

            if (bloodPictureData == null)
                return plotModel;

            var barSeries = new BarSeries
            {
                ItemsSource = new List<BarItem>(new[]
                {
                new BarItem{ Value = bloodPictureData[0], Color=OxyColors.DarkRed },
                new BarItem{ Value = bloodPictureData[1], Color=OxyColors.DarkRed },
                new BarItem{ Value = bloodPictureData[2], Color=OxyColors.DarkRed },
                new BarItem{ Value = bloodPictureData[3], Color=OxyColors.DarkRed },
                new BarItem{ Value = bloodPictureData[4] , Color=OxyColors.DarkRed},
                new BarItem{ Value = bloodPictureData[5] , Color=OxyColors.DarkRed}
                }),
                LabelPlacement = LabelPlacement.Inside,
            };

            plotModel.Series.Add(barSeries);

            return plotModel;
        }

        private void ButtonGetPlan_Click(object sender, EventArgs e)
        {
            switch(BloodState.Result1)
            {
                case nameof(Results.Hydrated):
                    textViewShouldDo1.Text = "Drink!!";
                    imageViewShuldDo1.SetImageResource(Resource.Drawable.drink);
                    break;
                case nameof(Results.Hurt):
                    textViewShouldDo1.Text = "Visit doctor";
                    imageViewShuldDo1.SetImageResource(Resource.Drawable.Medical);
                    break;
                case nameof(Results.Sick):
                    textViewShouldDo1.Text = "Visit doctor";
                    imageViewShuldDo1.SetImageResource(Resource.Drawable.Medical);
                    break;
                case nameof(Results.Kidney):
                    textViewShouldDo1.Text = "Visit doctor";
                    imageViewShuldDo1.SetImageResource(Resource.Drawable.Medical);
                    break;
                case nameof(Results.Oxygenation):
                    textViewShouldDo1.Text = "Go out!";
                    imageViewShuldDo1.SetImageResource(Resource.Drawable.running);
                    break;
            }

            if(BloodState.Result1 == BloodState.Result2)
            {
                textViewShouldDo2.Visibility = ViewStates.Invisible;
                imageViewShuldDo2.Visibility = ViewStates.Invisible;
                return;
            }
            else
            {
                textViewShouldDo2.Visibility = ViewStates.Visible;
                imageViewShuldDo2.Visibility = ViewStates.Visible;
            }

            switch (BloodState.Result2)
            {
                case nameof(Results.Hydrated):
                    textViewShouldDo2.Text = "Drink!!";
                    imageViewShuldDo2.SetImageResource(Resource.Drawable.drink);
                    break;
                case nameof(Results.Hurt):
                    textViewShouldDo2.Text = "Visit doctor";
                    imageViewShuldDo2.SetImageResource(Resource.Drawable.Medical);
                    break;
                case nameof(Results.Sick):
                    textViewShouldDo2.Text = "Visit doctor";
                    imageViewShuldDo2.SetImageResource(Resource.Drawable.Medical);
                    break;
                case nameof(Results.Kidney):
                    textViewShouldDo2.Text = "Visit doctor";
                    imageViewShuldDo2.SetImageResource(Resource.Drawable.Medical);
                    break;
                case nameof(Results.Oxygenation):
                    textViewShouldDo2.Text = "Go out!";
                    imageViewShuldDo2.SetImageResource(Resource.Drawable.running);
                    break;
            }
        }

        private void ButtonAnalyzeBP_Click(object sender, EventArgs e)
        {
            Analyze();
        }

        private void Analyze()
        {
            buttonGetPlan.Visibility = ViewStates.Visible;
            textViewAnalyzeName.Visibility = ViewStates.Visible;
            textViewAnalyzeResults.Visibility = ViewStates.Visible;
            textViewShouldDo.Visibility = ViewStates.Visible;

            var results = neuralNetwork.NeuralNetwork.Compute(bloodPictureData);
            BloodState = neuralNetwork.CheckYourState(results);


            textViewAnalyzeResults.Text = BloodState.Result1 + " : " + BloodState.Result2;
        }

        private void ButtonGetLastBP_Click(object sender, EventArgs e)
        {
            var Data = DatabaseAdapter.GetBloodData().Result;

            if(Data == null)
            Toast.MakeText(ApplicationContext, "This is your actual blood picture", ToastLength.Long).Show();
            else
            {
                BloodPictureGraph.Model = CalculateBP();
                BloodPictureGraph.Invalidate();
                Analyze();
            }
        }
    }

    sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            // For each assemblyName/typeName that you want to deserialize to
            // a different type, set typeToDeserialize to the desired type.
            String exeAssembly = Assembly.GetExecutingAssembly().FullName;


            // The following line of code returns the type.
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                typeName, exeAssembly));

            return typeToDeserialize;
        }
    }
}