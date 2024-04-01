using Android.Content;
using JavaUri = Android.Net.Uri;
using JavaFile = Java.IO.File;
using Android.Support.V4.Content;
using MystiqueNative.Droid.Utils;
using Android.Net;
using System;

namespace MystiqueNative.Droid.Helpers
{
    public class ContentResolverHelper
    {
        private const string PackageProvider = "com.GrupoRed.Fresco.provider";
        public static string ContentResolverToAbsolute(JavaUri contentPath)
        {
            var contentResolver = CurrentActivityDelegate.Instance.Activity.ContentResolver;

            var docId = "";
            using (var c1 = contentResolver.Query(contentPath, null, null, null, null))
            {
                c1.MoveToFirst();
                var documentId = c1.GetString(0);
                docId = documentId.Substring(documentId.LastIndexOf(":") + 1);
            }

            string absolutePath = null;

            const string selection = Android.Provider.MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
            using (var cursor = contentResolver.Query(Android.Provider.MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] { docId }, null))
            {
                if (cursor == null) return null;
                var columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                cursor.MoveToFirst();
                absolutePath = cursor.GetString(columnIndex);
            }
            return absolutePath;
        }

        public static JavaUri AbsoluteToContent(JavaFile file)
        {
            return FileProvider.GetUriForFile(CurrentActivityDelegate.Instance.Activity, PackageProvider, file);
        }

        internal static string ContentResolverToAbsolute(ContentResolver contentResolver, JavaUri data)
        {
            var docId = "";
            using (var c1 = contentResolver.Query(data, null, null, null, null))
            {
                c1.MoveToFirst();
                var documentId = c1.GetString(0);
                docId = documentId.Substring(documentId.LastIndexOf(":", StringComparison.Ordinal) + 1);
            }

            string absolutePath = null;

            var selection = Android.Provider.MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
            using (var cursor = contentResolver.Query(Android.Provider.MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] { docId }, null))
            {
                if (cursor == null) return null;
                var columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                cursor.MoveToFirst();
                absolutePath = cursor.GetString(columnIndex);
            }
            return absolutePath;
        }
    }
}