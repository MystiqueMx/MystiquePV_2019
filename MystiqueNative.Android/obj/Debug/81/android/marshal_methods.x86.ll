; ModuleID = 'obj\Debug\81\android\marshal_methods.x86.ll'
source_filename = "obj\Debug\81\android\marshal_methods.x86.ll"
target datalayout = "e-m:e-p:32:32-p270:32:32-p271:32:32-p272:64:64-f64:32:64-f80:32-n8:16:32-S128"
target triple = "i686-unknown-linux-android"


%struct.MonoImage = type opaque

%struct.MonoClass = type opaque

%struct.MarshalMethodsManagedClass = type {
	i32,; uint32_t token
	%struct.MonoClass*; MonoClass* klass
}

%struct.MarshalMethodName = type {
	i64,; uint64_t id
	i8*; char* name
}

%class._JNIEnv = type opaque

%class._jobject = type {
	i8; uint8_t b
}

%class._jclass = type {
	i8; uint8_t b
}

%class._jstring = type {
	i8; uint8_t b
}

%class._jthrowable = type {
	i8; uint8_t b
}

%class._jarray = type {
	i8; uint8_t b
}

%class._jobjectArray = type {
	i8; uint8_t b
}

%class._jbooleanArray = type {
	i8; uint8_t b
}

%class._jbyteArray = type {
	i8; uint8_t b
}

%class._jcharArray = type {
	i8; uint8_t b
}

%class._jshortArray = type {
	i8; uint8_t b
}

%class._jintArray = type {
	i8; uint8_t b
}

%class._jlongArray = type {
	i8; uint8_t b
}

%class._jfloatArray = type {
	i8; uint8_t b
}

%class._jdoubleArray = type {
	i8; uint8_t b
}

; assembly_image_cache
@assembly_image_cache = local_unnamed_addr global [0 x %struct.MonoImage*] zeroinitializer, align 4
; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = local_unnamed_addr constant [144 x i32] [
	i32 36068363, ; 0: Xamarin.Firebase.Perf.dll => 0x2265c0b => 52
	i32 39109920, ; 1: Newtonsoft.Json.dll => 0x254c520 => 19
	i32 85274472, ; 2: Com.OneSignal.Abstractions.dll => 0x5152f68 => 4
	i32 99966887, ; 3: Xamarin.Firebase.Iid.dll => 0x5f55fa7 => 51
	i32 166922606, ; 4: Xamarin.Android.Support.Compat.dll => 0x9f3096e => 29
	i32 172012715, ; 5: FastAndroidCamera.dll => 0xa40b4ab => 7
	i32 228478820, ; 6: Fresco.dll => 0xd9e4f64 => 0
	i32 232815796, ; 7: System.Web.Services => 0xde07cb4 => 70
	i32 266277096, ; 8: OneSignal.Android.Binding => 0xfdf10e8 => 20
	i32 275679612, ; 9: Humanizer => 0x106e897c => 14
	i32 293914992, ; 10: Xamarin.Android.Support.Transition => 0x1184c970 => 35
	i32 293936332, ; 11: Xamarin.GooglePlayServices.Auth.Api.Phone.dll => 0x11851ccc => 53
	i32 321597661, ; 12: System.Numerics => 0x132b30dd => 68
	i32 324489690, ; 13: Xamarin.Android.Support.Exif.dll => 0x135751da => 33
	i32 374925665, ; 14: Xamarin.Firebase.Analytics => 0x1658e961 => 48
	i32 389971796, ; 15: Xamarin.Android.Support.Core.UI => 0x173e7f54 => 30
	i32 463726861, ; 16: Xamarin.Facebook.AppLinks.Android => 0x1ba3e90d => 41
	i32 465846621, ; 17: mscorlib => 0x1bc4415d => 17
	i32 469710990, ; 18: System.dll => 0x1bff388e => 25
	i32 514659665, ; 19: Xamarin.Android.Support.Compat => 0x1ead1551 => 29
	i32 517857212, ; 20: Xamarin.Facebook.Common.Android => 0x1edddfbc => 42
	i32 520798577, ; 21: FFImageLoading.Platform => 0x1f0ac171 => 9
	i32 524195194, ; 22: Xamarin.Facebook.Places.Android.dll => 0x1f3e957a => 46
	i32 525008092, ; 23: SkiaSharp.dll => 0x1f4afcdc => 23
	i32 539750087, ; 24: Xamarin.Android.Support.Design => 0x202beec7 => 32
	i32 571524804, ; 25: Xamarin.Android.Support.v7.RecyclerView => 0x2210c6c4 => 39
	i32 589597883, ; 26: Xamarin.GooglePlayServices.Auth.Api.Phone => 0x23248cbb => 53
	i32 624688707, ; 27: Xamarin.Facebook.Messenger.Android => 0x253bfe43 => 45
	i32 644529652, ; 28: Com.OneSignal.Abstractions => 0x266abdf4 => 4
	i32 690569205, ; 29: System.Xml.Linq.dll => 0x29293ff5 => 27
	i32 698030881, ; 30: FFImageLoading.Transformations => 0x299b1b21 => 11
	i32 785976632, ; 31: MystiqueNative => 0x2ed90d38 => 18
	i32 898484977, ; 32: Xamarin.Firebase.Perf => 0x358dcaf1 => 52
	i32 922720838, ; 33: RestSharp.Serializers.Newtonsoft.Json.dll => 0x36ff9a46 => 22
	i32 955402788, ; 34: Newtonsoft.Json => 0x38f24a24 => 19
	i32 999372639, ; 35: Xamarin.Facebook.Core.Android => 0x3b91375f => 43
	i32 1014309828, ; 36: Xamarin.Firebase.Analytics.Impl => 0x3c7523c4 => 49
	i32 1098259244, ; 37: System => 0x41761b2c => 25
	i32 1134191450, ; 38: ZXingNetMobile.dll => 0x439a635a => 66
	i32 1193187932, ; 39: Xamarin.GooglePlayServices.Gcm.dll => 0x471e9a5c => 58
	i32 1218293947, ; 40: Xamarin.GooglePlayServices.Places.dll => 0x489db0bb => 62
	i32 1223560458, ; 41: MystiqueNative.dll => 0x48ee0d0a => 18
	i32 1324178973, ; 42: Fresco => 0x4eed5e1d => 0
	i32 1333047053, ; 43: Xamarin.Firebase.Common => 0x4f74af0d => 50
	i32 1359785034, ; 44: Xamarin.Android.Support.Design.dll => 0x510cac4a => 32
	i32 1445445088, ; 45: Xamarin.Android.Support.Fragment => 0x5627bde0 => 34
	i32 1571005899, ; 46: zxing.portable => 0x5da3a5cb => 65
	i32 1574652163, ; 47: Xamarin.Android.Support.Core.Utils.dll => 0x5ddb4903 => 31
	i32 1589711206, ; 48: Xamarin.Facebook.Places.Android => 0x5ec11166 => 46
	i32 1592978981, ; 49: System.Runtime.Serialization.dll => 0x5ef2ee25 => 69
	i32 1596265547, ; 50: Xamarin.GooglePlayServices.Iid.dll => 0x5f25144b => 59
	i32 1618541589, ; 51: Com.OneSignal.dll => 0x6078fc15 => 5
	i32 1635482189, ; 52: FFImageLoading.Transformations.dll => 0x617b7a4d => 11
	i32 1639515021, ; 53: System.Net.Http.dll => 0x61b9038d => 1
	i32 1776026572, ; 54: System.Core.dll => 0x69dc03cc => 24
	i32 1877418711, ; 55: Xamarin.Android.Support.v7.RecyclerView.dll => 0x6fe722d7 => 39
	i32 1904184254, ; 56: FastAndroidCamera => 0x717f8bbe => 7
	i32 1908813208, ; 57: Xamarin.GooglePlayServices.Basement => 0x71c62d98 => 57
	i32 1988263180, ; 58: Xamarin.GooglePlayServices.Gcm => 0x76827d0c => 58
	i32 2027186647, ; 59: Xamarin.Facebook.Login.Android => 0x78d469d7 => 44
	i32 2036392541, ; 60: Xamarin.Facebook.Android => 0x7960e25d => 40
	i32 2048146308, ; 61: Xamarin.Firebase.Analytics.dll => 0x7a143b84 => 48
	i32 2051717622, ; 62: Google.ZXing.Core.dll => 0x7a4ab9f6 => 13
	i32 2066383596, ; 63: FFImageLoading.Svg.Platform => 0x7b2a82ec => 10
	i32 2067982161, ; 64: Xamarin.Facebook.Common.Android.dll => 0x7b42e751 => 42
	i32 2095474518, ; 65: Xamarin.GooglePlayServices.Auth.Base => 0x7ce66756 => 54
	i32 2129483829, ; 66: Xamarin.GooglePlayServices.Base.dll => 0x7eed5835 => 56
	i32 2153805096, ; 67: Com.OneSignal => 0x80607528 => 5
	i32 2166116741, ; 68: Xamarin.Android.Support.Core.Utils => 0x811c5185 => 31
	i32 2201231467, ; 69: System.Net.Http => 0x8334206b => 1
	i32 2201706308, ; 70: Xamarin.Facebook.Messenger.Android.dll => 0x833b5f44 => 45
	i32 2286792735, ; 71: Xamarin.Facebook.AppLinks.Android.dll => 0x884db01f => 41
	i32 2329204181, ; 72: zxing.portable.dll => 0x8ad4d5d5 => 65
	i32 2330457430, ; 73: Xamarin.Android.Support.Core.UI.dll => 0x8ae7f556 => 30
	i32 2340826669, ; 74: FFImageLoading.dll => 0x8b862e2d => 8
	i32 2341995103, ; 75: ZXingNetMobile => 0x8b98025f => 66
	i32 2354643623, ; 76: Xamarin.Facebook.Share.Android.dll => 0x8c5902a7 => 47
	i32 2373288475, ; 77: Xamarin.Android.Support.Fragment.dll => 0x8d75821b => 34
	i32 2390616037, ; 78: Xamarin.Android.Support.Exif => 0x8e7de7e5 => 33
	i32 2431243866, ; 79: ZXing.Net.Mobile.Core.dll => 0x90e9d65a => 64
	i32 2475788418, ; 80: Java.Interop.dll => 0x93918882 => 15
	i32 2492959214, ; 81: Xamarin.Android.Support.v7.Preference.dll => 0x949789ee => 38
	i32 2686248867, ; 82: Xamarin.Facebook.Core.Android.dll => 0xa01ce7a3 => 43
	i32 2819470561, ; 83: System.Xml.dll => 0xa80db4e1 => 26
	i32 2847418871, ; 84: Xamarin.GooglePlayServices.Base => 0xa9b829f7 => 56
	i32 2870995654, ; 85: Xamarin.Firebase.Iid => 0xab1feac6 => 51
	i32 2873222696, ; 86: FFImageLoading => 0xab41e628 => 8
	i32 2905242038, ; 87: mscorlib.dll => 0xad2a79b6 => 17
	i32 3017076677, ; 88: Xamarin.GooglePlayServices.Maps => 0xb3d4efc5 => 61
	i32 3058099980, ; 89: Xamarin.GooglePlayServices.Tasks => 0xb646e70c => 63
	i32 3068715062, ; 90: Xamarin.Android.Arch.Lifecycle.Common => 0xb6e8e036 => 28
	i32 3071899978, ; 91: Xamarin.Firebase.Common.dll => 0xb719794a => 50
	i32 3111772706, ; 92: System.Runtime.Serialization => 0xb979e222 => 69
	i32 3168458674, ; 93: Xamarin.Facebook.Login.Android.dll => 0xbcdad7b2 => 44
	i32 3182640895, ; 94: Xamarin.Facebook.Android.dll => 0xbdb33eff => 40
	i32 3204380047, ; 95: System.Data.dll => 0xbefef58f => 67
	i32 3207837690, ; 96: Bolts.Tasks.dll => 0xbf33b7fa => 3
	i32 3230466174, ; 97: Xamarin.GooglePlayServices.Basement.dll => 0xc08d007e => 57
	i32 3247949154, ; 98: Mono.Security => 0xc197c562 => 71
	i32 3249260365, ; 99: RestSharp.dll => 0xc1abc74d => 21
	i32 3258802011, ; 100: OneSignal.Android.Binding.dll => 0xc23d5f5b => 20
	i32 3291879728, ; 101: Xamarin.Facebook.Share.Android => 0xc4361930 => 47
	i32 3304979567, ; 102: Humanizer.dll => 0xc4fdfc6f => 14
	i32 3317144872, ; 103: System.Data => 0xc5b79d28 => 67
	i32 3340387945, ; 104: SkiaSharp => 0xc71a4669 => 23
	i32 3366347497, ; 105: Java.Interop => 0xc8a662e9 => 15
	i32 3429136800, ; 106: System.Xml => 0xcc6479a0 => 26
	i32 3465947803, ; 107: Xamarin.GooglePlayServices.Auth.dll => 0xce962a9b => 55
	i32 3476120550, ; 108: Mono.Android => 0xcf3163e6 => 16
	i32 3494395880, ; 109: Xamarin.GooglePlayServices.Location.dll => 0xd0483fe8 => 60
	i32 3498942916, ; 110: Xamarin.Android.Support.v7.CardView.dll => 0xd08da1c4 => 37
	i32 3509114376, ; 111: System.Xml.Linq => 0xd128d608 => 27
	i32 3672681054, ; 112: Mono.Android.dll => 0xdae8aa5e => 16
	i32 3676310014, ; 113: System.Web.Services.dll => 0xdb2009fe => 70
	i32 3678221644, ; 114: Xamarin.Android.Support.v7.AppCompat => 0xdb3d354c => 36
	i32 3681174138, ; 115: Xamarin.Android.Arch.Lifecycle.Common.dll => 0xdb6a427a => 28
	i32 3691167503, ; 116: Xamarin.Android.Support.v7.Preference => 0xdc02bf0f => 38
	i32 3729924096, ; 117: Xamarin.GooglePlayServices.Auth => 0xde522000 => 55
	i32 3801609503, ; 118: RestSharp.Serializers.Newtonsoft.Json => 0xe297f51f => 22
	i32 3816437471, ; 119: RestSharp => 0xe37a36df => 21
	i32 3829621856, ; 120: System.Numerics.dll => 0xe4436460 => 68
	i32 3883175360, ; 121: Xamarin.Android.Support.v7.AppCompat.dll => 0xe7748dc0 => 36
	i32 3887061637, ; 122: FlexboxLayoutXamarinBindingAndroid.dll => 0xe7afda85 => 12
	i32 3912468689, ; 123: FFImageLoading.Svg.Platform.dll => 0xe93388d1 => 10
	i32 3948971440, ; 124: Bolts.AppLinks => 0xeb6085b0 => 2
	i32 3951554428, ; 125: Crashlytics.Droid => 0xeb87ef7c => 6
	i32 3967165417, ; 126: Xamarin.GooglePlayServices.Location => 0xec7623e9 => 60
	i32 3970018735, ; 127: Xamarin.GooglePlayServices.Tasks.dll => 0xeca1adaf => 63
	i32 3991193666, ; 128: Xamarin.GooglePlayServices.Auth.Base.dll => 0xede4c842 => 54
	i32 4000318121, ; 129: Google.ZXing.Core => 0xee7002a9 => 13
	i32 4045951918, ; 130: Crashlytics.Droid.dll => 0xf12853ae => 6
	i32 4068170347, ; 131: Xamarin.Firebase.Analytics.Impl.dll => 0xf27b5a6b => 49
	i32 4105002889, ; 132: Mono.Security.dll => 0xf4ad5f89 => 71
	i32 4128031935, ; 133: Xamarin.GooglePlayServices.Places => 0xf60cc4bf => 62
	i32 4128428945, ; 134: Bolts.AppLinks.dll => 0xf612d391 => 2
	i32 4151237749, ; 135: System.Core => 0xf76edc75 => 24
	i32 4184283386, ; 136: FFImageLoading.Platform.dll => 0xf96718fa => 9
	i32 4186595366, ; 137: ZXing.Net.Mobile.Core => 0xf98a6026 => 64
	i32 4216732326, ; 138: Xamarin.GooglePlayServices.Iid => 0xfb563aa6 => 59
	i32 4216993138, ; 139: Xamarin.Android.Support.Transition.dll => 0xfb5a3572 => 35
	i32 4219003402, ; 140: Xamarin.Android.Support.v7.CardView => 0xfb78e20a => 37
	i32 4238338748, ; 141: FlexboxLayoutXamarinBindingAndroid => 0xfc9feabc => 12
	i32 4258616067, ; 142: Bolts.Tasks => 0xfdd55303 => 3
	i32 4278134329 ; 143: Xamarin.GooglePlayServices.Maps.dll => 0xfeff2639 => 61
], align 4
@assembly_image_cache_indices = local_unnamed_addr constant [144 x i32] [
	i32 52, i32 19, i32 4, i32 51, i32 29, i32 7, i32 0, i32 70, ; 0..7
	i32 20, i32 14, i32 35, i32 53, i32 68, i32 33, i32 48, i32 30, ; 8..15
	i32 41, i32 17, i32 25, i32 29, i32 42, i32 9, i32 46, i32 23, ; 16..23
	i32 32, i32 39, i32 53, i32 45, i32 4, i32 27, i32 11, i32 18, ; 24..31
	i32 52, i32 22, i32 19, i32 43, i32 49, i32 25, i32 66, i32 58, ; 32..39
	i32 62, i32 18, i32 0, i32 50, i32 32, i32 34, i32 65, i32 31, ; 40..47
	i32 46, i32 69, i32 59, i32 5, i32 11, i32 1, i32 24, i32 39, ; 48..55
	i32 7, i32 57, i32 58, i32 44, i32 40, i32 48, i32 13, i32 10, ; 56..63
	i32 42, i32 54, i32 56, i32 5, i32 31, i32 1, i32 45, i32 41, ; 64..71
	i32 65, i32 30, i32 8, i32 66, i32 47, i32 34, i32 33, i32 64, ; 72..79
	i32 15, i32 38, i32 43, i32 26, i32 56, i32 51, i32 8, i32 17, ; 80..87
	i32 61, i32 63, i32 28, i32 50, i32 69, i32 44, i32 40, i32 67, ; 88..95
	i32 3, i32 57, i32 71, i32 21, i32 20, i32 47, i32 14, i32 67, ; 96..103
	i32 23, i32 15, i32 26, i32 55, i32 16, i32 60, i32 37, i32 27, ; 104..111
	i32 16, i32 70, i32 36, i32 28, i32 38, i32 55, i32 22, i32 21, ; 112..119
	i32 68, i32 36, i32 12, i32 10, i32 2, i32 6, i32 60, i32 63, ; 120..127
	i32 54, i32 13, i32 6, i32 49, i32 71, i32 62, i32 2, i32 24, ; 128..135
	i32 9, i32 64, i32 59, i32 35, i32 37, i32 12, i32 3, i32 61 ; 144..143
], align 4

@marshal_methods_number_of_classes = local_unnamed_addr constant i32 0, align 4

; marshal_methods_class_cache
@marshal_methods_class_cache = global [0 x %struct.MarshalMethodsManagedClass] [
], align 4; end of 'marshal_methods_class_cache' array


@get_function_pointer = internal unnamed_addr global void (i32, i32, i32, i8**)* null, align 4

; Function attributes: "frame-pointer"="none" "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" "stackrealign" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn writeonly
define void @xamarin_app_init (void (i32, i32, i32, i8**)* %fn) local_unnamed_addr #0
{
	store void (i32, i32, i32, i8**)* %fn, void (i32, i32, i32, i8**)** @get_function_pointer, align 4
	ret void
}

; Names of classes in which marshal methods reside
@mm_class_names = local_unnamed_addr constant [0 x i8*] zeroinitializer, align 4
@__MarshalMethodName_name.0 = internal constant [1 x i8] c"\00", align 1

; mm_method_names
@mm_method_names = local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	; 0
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		i8* getelementptr inbounds ([1 x i8], [1 x i8]* @__MarshalMethodName_name.0, i32 0, i32 0); name
	}
], align 8; end of 'mm_method_names' array


attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable willreturn writeonly "frame-pointer"="none" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" "stackrealign" }
attributes #1 = { "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable "frame-pointer"="none" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" "stackrealign" }
attributes #2 = { nounwind }

!llvm.module.flags = !{!0, !1, !2}
!llvm.ident = !{!3}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!2 = !{i32 1, !"NumRegisterParameters", i32 0}
!3 = !{!"Xamarin.Android remotes/origin/d17-5 @ 45b0e144f73b2c8747d8b5ec8cbd3b55beca67f0"}
!llvm.linker.options = !{}
