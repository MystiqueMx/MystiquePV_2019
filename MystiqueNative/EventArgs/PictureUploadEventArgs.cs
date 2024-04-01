using System;

namespace MystiqueNative.ViewModels
{
    public class PictureUploadEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}