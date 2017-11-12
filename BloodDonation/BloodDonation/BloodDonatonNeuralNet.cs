using AForge.Neuro;
using System;

namespace BloodDonation
{
    [Serializable]
    public class BloodDonatonNeuralNet
    {
        public ActivationNetwork NeuralNetwork { get; set; }
        public BloodDonatonNeuralNet()
        {
            
        }

        public BloodState CheckYourState(double[] output)
        {
            BloodState state = new BloodState();

            if (output[0] < 0.05)
                state.Result1 = nameof(Results.Hydrated);
            if (output[0] > 0.05 && output[0] < 0.15)
                state.Result1 = nameof(Results.Hurt);
            if (output[0] > 0.15 && output[0] < 0.25)
                state.Result1 = nameof(Results.Kidney);
            if (output[0] > 0.25 && output[0] < 0.35)
                state.Result1 = nameof(Results.Oxygenation);
            if (output[0] > 0.35)
                state.Result1 = nameof(Results.Sick);

            if (output[1] < 0.05)
                state.Result2 = nameof(Results.Hydrated);
            if (output[1] > 0.05 && output[0] < 0.15)
                state.Result2 = nameof(Results.Hurt);
            if (output[1] > 0.15 && output[0] < 0.25)
                state.Result2 = nameof(Results.Kidney);
            if (output[1] > 0.25 && output[0] < 0.35)
                state.Result2 = nameof(Results.Oxygenation);
            if (output[1] > 0.35)
                state.Result2 = nameof(Results.Sick);

            return state;
        }

        public class BloodState
        {
            public string Result1 { get; set; }
            public string Result2 { get; set; }
        }
    }
}
