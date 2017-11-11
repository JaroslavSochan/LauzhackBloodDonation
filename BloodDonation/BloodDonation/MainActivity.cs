using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Threading.Tasks;

namespace BloodDonation
{
    [Activity(Label = "BloodDonation", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public EditText editTextName;
        public EditText editTextPass;
        public Button buttonLogin;
        public ProgressDialog progressDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            editTextName = FindViewById<EditText>(Resource.Id.editTextName);
            editTextPass = FindViewById<EditText>(Resource.Id.editTextPass);

            progressDialog = new ProgressDialog(this);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.SetMessage("Wait...");
            progressDialog.SetCancelable(false);
            progressDialog.Create();


            buttonLogin = FindViewById<Button>(Resource.Id.buttonLogin);
            buttonLogin.Click += ButtonLogin_Click;
        }

        private async void ButtonLogin_Click(object sender, System.EventArgs e)
        {
            Credentials credentials = new Credentials() { User = editTextName.Text, Pass = editTextPass.Text };
            Person person = new Person();
            progressDialog.Show();
            await Task.Delay(300);
            await DatabaseAdapter.LogIn(credentials, person);
            progressDialog.Hide();
            if (person == null)
            {
                //textViewError.Text = "Error";
            }
            else
            {
                //textViewError.Text = "Success";

                Intent MainMenuIntent = new Intent();
                //MainMenuIntent.SetClass(this, typeof(MainMenuActivity));
                //StartActivity(MainMenuIntent);
            }
        }
    }
}

