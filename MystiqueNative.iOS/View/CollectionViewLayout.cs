using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class CollectionViewLayout : UICollectionViewDelegateFlowLayout
    {
        
        
        public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            
            return base.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
        }

        public CollectionViewLayout (IntPtr handle) : base (handle)
        {
        }
    }
}