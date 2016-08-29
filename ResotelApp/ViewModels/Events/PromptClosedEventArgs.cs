using System;

namespace ResotelApp.ViewModels.Events
{
    class PromptClosedEventArgs : EventArgs
    {
        public string PromptResult { get; set; }

        public PromptClosedEventArgs(string result)
        {
            PromptResult = result;
        }
    }
}