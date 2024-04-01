using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVFoundation;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using System.Runtime.Serialization;

namespace MystiqueNative.iOS.Helpers.Xamarin_Essentials
{
    public static partial class Flashlight
    {
        static Task PlatformTurnOnAsync()
        {
            Toggle(true);

            return Task.CompletedTask;
        }

        static Task PlatformTurnOffAsync()
        {
            Toggle(false);

            return Task.CompletedTask;
        }

        static void Toggle(bool on)
        {
            var captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            if (captureDevice == null || !(captureDevice.HasFlash || captureDevice.HasTorch))
                throw new FeatureNotSupportedException();

            captureDevice.LockForConfiguration(out var error);

            if (error == null)
            {
                if (on)
                {
                    if (captureDevice.HasTorch)
                        captureDevice.SetTorchModeLevel(AVCaptureDevice.MaxAvailableTorchLevel, out var torchErr);
                    if (captureDevice.HasFlash)
                        captureDevice.FlashMode = AVCaptureFlashMode.On;
                }
                else
                {
                    if (captureDevice.HasTorch)
                        captureDevice.TorchMode = AVCaptureTorchMode.Off;
                    if (captureDevice.HasFlash)
                        captureDevice.FlashMode = AVCaptureFlashMode.Off;
                }
            }

            captureDevice.UnlockForConfiguration();
            captureDevice.Dispose();
            captureDevice = null;
        }

        [Serializable]
        private class FeatureNotSupportedException : Exception
        {
            public FeatureNotSupportedException()
            {
            }

            public FeatureNotSupportedException(string message) : base(message)
            {
            }

            public FeatureNotSupportedException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected FeatureNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}