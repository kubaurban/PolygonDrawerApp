using System;

namespace Project_1.Helpers.UI
{
    public class SaveLengthArgs : EventArgs
    {
        public int Length { get; private set; }
        public SaveLengthArgs(int length)
        {
            Length = length;
        }
    }
}
