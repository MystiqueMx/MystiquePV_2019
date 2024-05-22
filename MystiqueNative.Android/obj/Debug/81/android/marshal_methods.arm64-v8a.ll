; ModuleID = 'obj\Debug\81\android\marshal_methods.arm64-v8a.ll'
source_filename = "obj\Debug\81\android\marshal_methods.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android"


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
@assembly_image_cache = local_unnamed_addr global [0 x %struct.MonoImage*] zeroinitializer, align 8
; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = local_unnamed_addr constant [144 x i64] [
	i64 120698629574877762, ; 0: Mono.Android => 0x1accec39cafe242 => 16
	i64 173973121530698583, ; 1: Bolts.Tasks => 0x26a13a1dd4d9b57 => 3
	i64 182106475126841455, ; 2: Humanizer.dll => 0x286f8dfd13be06f => 14
	i64 322890146361922204, ; 3: Xamarin.Android.Support.Exif => 0x47b22e09b60529c => 33
	i64 327827996457880672, ; 4: Bolts.AppLinks.dll => 0x48cadd36c4ccc60 => 2
	i64 435170709725415398, ; 5: Xamarin.GooglePlayServices.Location => 0x60a097471d687e6 => 60
	i64 554274502437711452, ; 6: Xamarin.Android.Support.v7.Preference.dll => 0x7b12db929db2e5c => 38
	i64 687654259221141486, ; 7: Xamarin.GooglePlayServices.Base => 0x98b09e7c92917ee => 56
	i64 950350744942731166, ; 8: RestSharp.Serializers.Newtonsoft.Json => 0xd3052f7a456df9e => 22
	i64 1000557547492888992, ; 9: Mono.Security.dll => 0xde2b1c9cba651a0 => 71
	i64 1031345934883873950, ; 10: Com.OneSignal.Abstractions.dll => 0xe5013a9d94be49e => 4
	i64 1179379953299071679, ; 11: Xamarin.Facebook.Android => 0x105dffd1a39ba2bf => 40
	i64 1342439039765371018, ; 12: Xamarin.Android.Support.Fragment => 0x12a14d31b1d4d88a => 34
	i64 1425944114962822056, ; 13: System.Runtime.Serialization.dll => 0x13c9f89e19eaf3a8 => 69
	i64 1648353987601573233, ; 14: Xamarin.Facebook.Login.Android => 0x16e02137e7172571 => 44
	i64 1731380447121279447, ; 15: Newtonsoft.Json => 0x18071957e9b889d7 => 19
	i64 1744702963312407042, ; 16: Xamarin.Android.Support.v7.AppCompat => 0x18366e19eeceb202 => 36
	i64 1839263476738264739, ; 17: Xamarin.Facebook.AppLinks.Android => 0x1986606323558ea3 => 41
	i64 1860886102525309849, ; 18: Xamarin.Android.Support.v7.RecyclerView.dll => 0x19d3320d047b7399 => 39
	i64 1984538867944326539, ; 19: FFImageLoading.Platform.dll => 0x1b8a7f95fac7058b => 9
	i64 2133195048986300728, ; 20: Newtonsoft.Json.dll => 0x1d9aa1984b735138 => 19
	i64 2136857779576739683, ; 21: Xamarin.GooglePlayServices.Places.dll => 0x1da7a4d437339763 => 62
	i64 2211319766525259838, ; 22: Xamarin.Facebook.Places.Android.dll => 0x1eb02f9c9089b03e => 46
	i64 2284400282711631002, ; 23: System.Web.Services => 0x1fb3d1f42fd4249a => 70
	i64 2389500855569724380, ; 24: Fresco => 0x2129365e36b9b3dc => 0
	i64 2592350477072141967, ; 25: System.Xml.dll => 0x23f9e10627330e8f => 26
	i64 2624866290265602282, ; 26: mscorlib.dll => 0x246d65fbde2db8ea => 17
	i64 2912204874766033162, ; 27: Com.OneSignal => 0x286a3ae77197210a => 5
	i64 2954987430423977617, ; 28: Xamarin.GooglePlayServices.Auth.Base => 0x290239696a2a5e91 => 54
	i64 3237841904867657797, ; 29: Xamarin.GooglePlayServices.Iid.dll => 0x2cef200ec282c845 => 59
	i64 3245892109222566308, ; 30: Humanizer => 0x2d0bb9ad057459a4 => 14
	i64 3364695309916733813, ; 31: Xamarin.Firebase.Common => 0x2eb1cc8eb5028175 => 50
	i64 3385874954764959122, ; 32: Com.OneSignal.Abstractions => 0x2efd0b550ca83d92 => 4
	i64 3411255996856937470, ; 33: Xamarin.GooglePlayServices.Basement => 0x2f5737416a942bfe => 57
	i64 3427548605411023127, ; 34: Xamarin.GooglePlayServices.Auth.Api.Phone.dll => 0x2f91194bf3e8d917 => 53
	i64 3506890173557649318, ; 35: FlexboxLayoutXamarinBindingAndroid => 0x30aafa0855362fa6 => 12
	i64 3531994851595924923, ; 36: System.Numerics => 0x31042a9aade235bb => 68
	i64 3706843889277265173, ; 37: Xamarin.Facebook.Common.Android.dll => 0x33715ae0aa32e515 => 42
	i64 4247996603072512073, ; 38: Xamarin.GooglePlayServices.Tasks => 0x3af3ea6755340049 => 63
	i64 4255796613242758200, ; 39: zxing.portable => 0x3b0fa078b8a52438 => 65
	i64 4264996749430196783, ; 40: Xamarin.Android.Support.Transition.dll => 0x3b304ff259fb8a2f => 35
	i64 4292233171264798357, ; 41: ZXing.Net.Mobile.Core.dll => 0x3b911353fa62fe95 => 64
	i64 4914391832395879945, ; 42: FFImageLoading.Transformations => 0x44336d5581099609 => 11
	i64 5188846920129460862, ; 43: Com.OneSignal.dll => 0x48027cc83c323a7e => 5
	i64 5195665324143414425, ; 44: Xamarin.Facebook.Android.dll => 0x481ab615a1620c99 => 40
	i64 5233983725610684227, ; 45: FastAndroidCamera => 0x48a2d877b5334f43 => 7
	i64 5258427006098912452, ; 46: Xamarin.GooglePlayServices.Auth.Base.dll => 0x48f9af806fd8b4c4 => 54
	i64 5503058224226985080, ; 47: Xamarin.Facebook.Share.Android => 0x4c5eca4c69576478 => 47
	i64 5507995362134886206, ; 48: System.Core.dll => 0x4c705499688c873e => 24
	i64 5750603425310116216, ; 49: Crashlytics.Droid.dll => 0x4fce3f58e9c10978 => 6
	i64 5767749323661124970, ; 50: ZXing.Net.Mobile.Core => 0x500b29737652256a => 64
	i64 5832372969002002972, ; 51: MystiqueNative.dll => 0x50f0c05066f46a1c => 18
	i64 5868750392354936134, ; 52: Google.ZXing.Core.dll => 0x5171fd634bc01546 => 13
	i64 6167443447940383130, ; 53: OneSignal.Android.Binding.dll => 0x55972923aecc019a => 20
	i64 6300241346327543539, ; 54: Xamarin.Firebase.Iid => 0x576ef41fd714fef3 => 51
	i64 6568694463393773960, ; 55: Xamarin.Facebook.Core.Android => 0x5b28b0cfe0a93988 => 43
	i64 6591024623626361694, ; 56: System.Web.Services.dll => 0x5b7805f9751a1b5e => 70
	i64 6671798237668743565, ; 57: SkiaSharp => 0x5c96fd260152998d => 23
	i64 6876862101832370452, ; 58: System.Xml.Linq => 0x5f6f85a57d108914 => 27
	i64 7007997260651418532, ; 59: Xamarin.Firebase.Analytics.dll => 0x61416860ec09d3a4 => 48
	i64 7141281584637745974, ; 60: Xamarin.GooglePlayServices.Maps.dll => 0x631aedc3dd5f1b36 => 61
	i64 7295207093891867425, ; 61: Xamarin.Android.Support.v7.Preference => 0x653dc8334255a321 => 38
	i64 7488575175965059935, ; 62: System.Xml.Linq.dll => 0x67ecc3724534ab5f => 27
	i64 7546770571164721818, ; 63: FlexboxLayoutXamarinBindingAndroid.dll => 0x68bb83d997a5229a => 12
	i64 7654504624184590948, ; 64: System.Net.Http => 0x6a3a4366801b8264 => 1
	i64 7820441508502274321, ; 65: System.Data => 0x6c87ca1e14ff8111 => 67
	i64 7879037620440914030, ; 66: Xamarin.Android.Support.v7.AppCompat.dll => 0x6d57f6f88a51d86e => 36
	i64 8009580930144105919, ; 67: Xamarin.Facebook.Common.Android => 0x6f27bf6b5cfb75bf => 42
	i64 8167236081217502503, ; 68: Java.Interop.dll => 0x7157d9f1a9b8fd27 => 15
	i64 8205064624148743999, ; 69: OneSignal.Android.Binding => 0x71de3ecda982873f => 20
	i64 8308752701911091152, ; 70: Xamarin.GooglePlayServices.Gcm.dll => 0x734e9e8f8bfa47d0 => 58
	i64 8428678171113854126, ; 71: Xamarin.Firebase.Iid.dll => 0x74f8ae23bb5494ae => 51
	i64 8450804802111199978, ; 72: Xamarin.Android.Support.Exif.dll => 0x75474a317800daea => 33
	i64 8626175481042262068, ; 73: Java.Interop => 0x77b654e585b55834 => 15
	i64 8875563461781360128, ; 74: Xamarin.Facebook.Core.Android.dll => 0x7b2c55f198ea0e00 => 43
	i64 8929323300925261055, ; 75: Xamarin.GooglePlayServices.Iid => 0x7beb543b8c44c4ff => 59
	i64 9114191852432800567, ; 76: FFImageLoading.dll => 0x7e7c1d3363043b37 => 8
	i64 9475595603812259686, ; 77: Xamarin.Android.Support.Design => 0x838013ff707b9766 => 32
	i64 9662334977499516867, ; 78: System.Numerics.dll => 0x8617827802b0cfc3 => 68
	i64 9808709177481450983, ; 79: Mono.Android.dll => 0x881f890734e555e7 => 16
	i64 9866412715007501892, ; 80: Xamarin.Android.Arch.Lifecycle.Common.dll => 0x88ec8a16fd6b6644 => 28
	i64 9875200773399460291, ; 81: Xamarin.GooglePlayServices.Base.dll => 0x890bc2c8482339c3 => 56
	i64 9998632235833408227, ; 82: Mono.Security => 0x8ac2470b209ebae3 => 71
	i64 10038780035334861115, ; 83: System.Net.Http.dll => 0x8b50e941206af13b => 1
	i64 10344691795329379848, ; 84: Xamarin.Firebase.Analytics.Impl.dll => 0x8f8fba611b816e08 => 49
	i64 10523025093015579444, ; 85: Xamarin.GooglePlayServices.Gcm => 0x92094b9197b72b34 => 58
	i64 10572378644154332435, ; 86: RestSharp.Serializers.Newtonsoft.Json.dll => 0x92b8a25cabbeb513 => 22
	i64 11021508495176037734, ; 87: Xamarin.Firebase.Perf.dll => 0x98f44394f7eaf566 => 52
	i64 11023048688141570732, ; 88: System.Core => 0x98f9bc61168392ac => 24
	i64 11037814507248023548, ; 89: System.Xml => 0x992e31d0412bf7fc => 26
	i64 11201462017695523848, ; 90: FFImageLoading.Transformations.dll => 0x9b73965b71c5a408 => 11
	i64 11242276177335301618, ; 91: Google.ZXing.Core => 0x9c04969e80e835f2 => 13
	i64 11376461258732682436, ; 92: Xamarin.Android.Support.Compat => 0x9de14f3d5fc13cc4 => 29
	i64 11395105072750394936, ; 93: Xamarin.Android.Support.v7.CardView => 0x9e238bb09789fe38 => 37
	i64 11683710219442713716, ; 94: ZXingNetMobile => 0xa224e08aa87bf474 => 66
	i64 12013962889899020729, ; 95: Xamarin.GooglePlayServices.Auth => 0xa6ba2b987d2811b9 => 55
	i64 12326896518204651517, ; 96: Bolts.AppLinks => 0xab11ef12969677fd => 2
	i64 12336928085371509187, ; 97: Xamarin.GooglePlayServices.Auth.Api.Phone => 0xab3592bad41bd9c3 => 53
	i64 12414299427252656003, ; 98: Xamarin.Android.Support.Compat.dll => 0xac48738e28bad783 => 29
	i64 12415529026387440552, ; 99: Xamarin.Firebase.Analytics.Impl => 0xac4cd1de845ff3a8 => 49
	i64 12528960204261615995, ; 100: Xamarin.GooglePlayServices.Places => 0xaddfceecac04e97b => 62
	i64 12559163541709922900, ; 101: Xamarin.Android.Support.v7.CardView.dll => 0xae4b1cb32ba82254 => 37
	i64 12845046283116214416, ; 102: Xamarin.Firebase.Analytics => 0xb242c589dc97f890 => 48
	i64 12952608645614506925, ; 103: Xamarin.Android.Support.Core.Utils => 0xb3c0e8eff48193ad => 31
	i64 13358059602087096138, ; 104: Xamarin.Android.Support.Fragment.dll => 0xb9615c6f1ee5af4a => 34
	i64 13370592475155966277, ; 105: System.Runtime.Serialization => 0xb98de304062ea945 => 69
	i64 13647894001087880694, ; 106: System.Data.dll => 0xbd670f48cb071df6 => 67
	i64 14164783236351491542, ; 107: FFImageLoading.Svg.Platform.dll => 0xc4936b4e23237dd6 => 10
	i64 14400856865250966808, ; 108: Xamarin.Android.Support.Core.UI => 0xc7da1f051a877d18 => 30
	i64 14438260825521943376, ; 109: RestSharp.dll => 0xc85f01b93fac7350 => 21
	i64 14678510994762383812, ; 110: Xamarin.GooglePlayServices.Location.dll => 0xcbb48bfaca7a41c4 => 60
	i64 14685058019264274580, ; 111: Xamarin.Facebook.Messenger.Android => 0xcbcbce76b83fa094 => 45
	i64 14707807605605968470, ; 112: MystiqueNative => 0xcc1ca1178ceeaa56 => 18
	i64 14789919016435397935, ; 113: Xamarin.Firebase.Common.dll => 0xcd4058fc2f6d352f => 50
	i64 15059979474818870170, ; 114: Xamarin.Facebook.Share.Android.dll => 0xd0ffcb8a8940ab9a => 47
	i64 15404322903526314552, ; 115: FFImageLoading.Svg.Platform => 0xd5c72610ae199238 => 10
	i64 15418891414777631748, ; 116: Xamarin.Android.Support.Transition => 0xd5fae80c88241404 => 35
	i64 15609085926864131306, ; 117: System.dll => 0xd89e9cf3334914ea => 25
	i64 15851975962649584118, ; 118: zxing.portable.dll => 0xdbfd882691c261f6 => 65
	i64 15857316427823063929, ; 119: Xamarin.Facebook.Login.Android.dll => 0xdc108146835c2379 => 44
	i64 15930129725311349754, ; 120: Xamarin.GooglePlayServices.Tasks.dll => 0xdd1330956f12f3fa => 63
	i64 16087062485533818684, ; 121: Xamarin.Firebase.Perf => 0xdf40ba1901c17f3c => 52
	i64 16107354805249926211, ; 122: ZXingNetMobile.dll => 0xdf88d1dade1a6443 => 66
	i64 16119456071779071829, ; 123: FastAndroidCamera.dll => 0xdfb3cfe48ae7b755 => 7
	i64 16154507427712707110, ; 124: System => 0xe03056ea4e39aa26 => 25
	i64 16324796876805858114, ; 125: SkiaSharp.dll => 0xe28d5444586b6342 => 23
	i64 16395018361961052754, ; 126: Bolts.Tasks.dll => 0xe386ce55eeb1fa52 => 3
	i64 16426381707289907419, ; 127: Xamarin.Facebook.Places.Android => 0xe3f63b21cffd50db => 46
	i64 16833383113903931215, ; 128: mscorlib => 0xe99c30c1484d7f4f => 17
	i64 16932527889823454152, ; 129: Xamarin.Android.Support.Core.Utils.dll => 0xeafc6c67465253c8 => 31
	i64 17151170952569239713, ; 130: RestSharp => 0xee05331c4de338a1 => 21
	i64 17263484459350267324, ; 131: Fresco.dll => 0xef9437a6610c95bc => 0
	i64 17286476602864070850, ; 132: Xamarin.Facebook.Messenger.Android.dll => 0xefe5e6e1f3d070c2 => 45
	i64 17310799966561153083, ; 133: Xamarin.GooglePlayServices.Auth.dll => 0xf03c50da60b8b03b => 55
	i64 17428701562824544279, ; 134: Xamarin.Android.Support.Core.UI.dll => 0xf1df2fbaec73d017 => 30
	i64 17643123953373031521, ; 135: FFImageLoading => 0xf4d8f7c220fc2c61 => 8
	i64 17760961058993581169, ; 136: Xamarin.Android.Arch.Lifecycle.Common => 0xf67b9bfb46dbac71 => 28
	i64 17936749993673010118, ; 137: Xamarin.Android.Support.Design.dll => 0xf8ec231615deabc6 => 32
	i64 17947624217716767869, ; 138: FFImageLoading.Platform => 0xf912c522ab34bc7d => 9
	i64 17969331831154222830, ; 139: Xamarin.GooglePlayServices.Maps => 0xf95fe418471126ee => 61
	i64 17986907704309214542, ; 140: Xamarin.GooglePlayServices.Basement.dll => 0xf99e554223166d4e => 57
	i64 18090425465832348288, ; 141: Xamarin.Android.Support.v7.RecyclerView => 0xfb0e1a1d2e9e1a80 => 39
	i64 18145376319941272927, ; 142: Crashlytics.Droid => 0xfbd1539fe9e2655f => 6
	i64 18396660829597971126 ; 143: Xamarin.Facebook.AppLinks.Android.dll => 0xff4e118e0987b2b6 => 41
], align 8
@assembly_image_cache_indices = local_unnamed_addr constant [144 x i32] [
	i32 16, i32 3, i32 14, i32 33, i32 2, i32 60, i32 38, i32 56, ; 0..7
	i32 22, i32 71, i32 4, i32 40, i32 34, i32 69, i32 44, i32 19, ; 8..15
	i32 36, i32 41, i32 39, i32 9, i32 19, i32 62, i32 46, i32 70, ; 16..23
	i32 0, i32 26, i32 17, i32 5, i32 54, i32 59, i32 14, i32 50, ; 24..31
	i32 4, i32 57, i32 53, i32 12, i32 68, i32 42, i32 63, i32 65, ; 32..39
	i32 35, i32 64, i32 11, i32 5, i32 40, i32 7, i32 54, i32 47, ; 40..47
	i32 24, i32 6, i32 64, i32 18, i32 13, i32 20, i32 51, i32 43, ; 48..55
	i32 70, i32 23, i32 27, i32 48, i32 61, i32 38, i32 27, i32 12, ; 56..63
	i32 1, i32 67, i32 36, i32 42, i32 15, i32 20, i32 58, i32 51, ; 64..71
	i32 33, i32 15, i32 43, i32 59, i32 8, i32 32, i32 68, i32 16, ; 72..79
	i32 28, i32 56, i32 71, i32 1, i32 49, i32 58, i32 22, i32 52, ; 80..87
	i32 24, i32 26, i32 11, i32 13, i32 29, i32 37, i32 66, i32 55, ; 88..95
	i32 2, i32 53, i32 29, i32 49, i32 62, i32 37, i32 48, i32 31, ; 96..103
	i32 34, i32 69, i32 67, i32 10, i32 30, i32 21, i32 60, i32 45, ; 104..111
	i32 18, i32 50, i32 47, i32 10, i32 35, i32 25, i32 65, i32 44, ; 112..119
	i32 63, i32 52, i32 66, i32 7, i32 25, i32 23, i32 3, i32 46, ; 120..127
	i32 17, i32 31, i32 21, i32 0, i32 45, i32 55, i32 30, i32 8, ; 128..135
	i32 28, i32 32, i32 9, i32 61, i32 57, i32 39, i32 6, i32 41 ; 144..143
], align 4

@marshal_methods_number_of_classes = local_unnamed_addr constant i32 0, align 4

; marshal_methods_class_cache
@marshal_methods_class_cache = global [0 x %struct.MarshalMethodsManagedClass] [
], align 8; end of 'marshal_methods_class_cache' array


@get_function_pointer = internal unnamed_addr global void (i32, i32, i32, i8**)* null, align 8

; Function attributes: "frame-pointer"="non-leaf" "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+neon,+outline-atomics" uwtable willreturn writeonly
define void @xamarin_app_init (void (i32, i32, i32, i8**)* %fn) local_unnamed_addr #0
{
	store void (i32, i32, i32, i8**)* %fn, void (i32, i32, i32, i8**)** @get_function_pointer, align 8
	ret void
}

; Names of classes in which marshal methods reside
@mm_class_names = local_unnamed_addr constant [0 x i8*] zeroinitializer, align 8
@__MarshalMethodName_name.0 = internal constant [1 x i8] c"\00", align 1

; mm_method_names
@mm_method_names = local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	; 0
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		i8* getelementptr inbounds ([1 x i8], [1 x i8]* @__MarshalMethodName_name.0, i32 0, i32 0); name
	}
], align 8; end of 'mm_method_names' array


attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable willreturn writeonly "frame-pointer"="non-leaf" "target-cpu"="generic" "target-features"="+neon,+outline-atomics" }
attributes #1 = { "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable "frame-pointer"="non-leaf" "target-cpu"="generic" "target-features"="+neon,+outline-atomics" }
attributes #2 = { nounwind }

!llvm.module.flags = !{!0, !1, !2, !3, !4, !5}
!llvm.ident = !{!6}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!2 = !{i32 1, !"branch-target-enforcement", i32 0}
!3 = !{i32 1, !"sign-return-address", i32 0}
!4 = !{i32 1, !"sign-return-address-all", i32 0}
!5 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
!6 = !{!"Xamarin.Android remotes/origin/d17-5 @ 45b0e144f73b2c8747d8b5ec8cbd3b55beca67f0"}
!llvm.linker.options = !{}
