using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Android;
using System;
using System.Collections.Generic;
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

        private Button buttonGetLastBP;
        private Button buttonAnalyzeBP;
        private Button buttonGetPlan;

        private ProgressDialog progressDialog;

        private PlotView BloodPictureGraph;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainMenu);
            // Create your application here

            textViewLastBloodPicture = FindViewById<TextView>(Resource.Id.textViewLastBloodPicture);
            textViewAnalyzeName = FindViewById<TextView>(Resource.Id.textViewAnalyzeName);
            textViewAnalyzeResults = FindViewById<TextView>(Resource.Id.textViewAnalyzeResults);
            textViewShouldDo = FindViewById<TextView>(Resource.Id.textViewShouldDo);

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

            BloodPictureGraph.Model = CalculateBP();
            progressDialog.Hide();

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
                "Apple cake",
                "Baumkuchen",
                "Bundt Cake",
                "Chocolate cake",
                "Carrot cake",
                "Bundt Cakeda",
                "Chocolate casadke",
                "Carrot casadke"
        }
                //Minimum = 0,
                //Maximum = 15
            });

            var barSeries = new BarSeries
            {
                ItemsSource = new List<BarItem>(new[]
                {
                new BarItem{ Value = (1.0), Color=OxyColors.Purple },
                new BarItem{ Value = (2.0), Color=OxyColors.DarkRed },
                new BarItem{ Value = (5.0), Color=OxyColors.IndianRed },
                new BarItem{ Value = (7.0), Color=OxyColors.PaleVioletRed },
                new BarItem{ Value = (1.0) , Color=OxyColors.OrangeRed},
                new BarItem{ Value = (12.0), Color=OxyColors.Red },
                new BarItem{ Value = (6.0) , Color=OxyColors.Red},
                new BarItem{ Value = (9.0) , Color=OxyColors.Red}
                }),
                LabelPlacement = LabelPlacement.Inside,
            };

            plotModel.Series.Add(barSeries);

            return plotModel;
        }

        private void ButtonGetPlan_Click(object sender, EventArgs e)
        {
            
        }

        private void ButtonAnalyzeBP_Click(object sender, EventArgs e)
        {
            buttonGetPlan.Visibility = ViewStates.Visible;
            textViewAnalyzeName.Visibility = ViewStates.Visible;
            textViewAnalyzeResults.Visibility = ViewStates.Visible;
            textViewShouldDo.Visibility = ViewStates.Visible;
        }

        private void ButtonGetLastBP_Click(object sender, EventArgs e)
        {
            Toast.MakeText(ApplicationContext, "This is your actual blood picture", ToastLength.Long).Show();
        }
    }
}