echo "restoring nuget packages"
nuget restore "Netlenium/Netlenium.sln"

echo "deleting old builds"
rm -rf "Build/Netlenium/linux_release_x64"
rm -rf "Build/Netlenium/linux_release_x86"
rm -rf "Build/Netlenium/linux_release_anycpu"
rm -rf "Build/Netlenium/linux_debug_x64"
rm -rf "Build/Netlenium/linux_debug_x86"
rm -rf "Build/Netlenium/linux_debug_anycpu"

echo "building x64 release using msbuild"
msbuild /p:Configuration="Linux Release" /p:Platform="x64" "Netlenium/Netlenium.sln" /verbosity:diagnostic -t:build
echo "building x86 release using msbuild"
msbuild /p:Configuration="Linux Release" /p:Platform="x86" "Netlenium/Netlenium.sln" /verbosity:diagnostic -t:build
echo "building ARM release using msbuild"
msbuild /p:Configuration="Linux Release" /p:Platform="Any CPU" "Netlenium/Netlenium.sln" /verbosity:diagnostic -t:build

echo "building x64 debug using msbuild"
msbuild /p:Configuration="Linux Debug" /p:Platform="x64" "Netlenium/Netlenium.sln" /verbosity:diagnostic -t:build
echo "building x86 debug using msbuild"
msbuild /p:Configuration="Linux Debug" /p:Platform="x86" "Netlenium/Netlenium.sln" /verbosity:diagnostic -t:build
echo "building ARM debug using msbuild"
msbuild /p:Configuration="Linux Debug" /p:Platform="Any CPU" "Netlenium/Netlenium.sln" /verbosity:diagnostic -t:build


echo "changing current directory to 'linux_release_x64'"
cd "Build/Netlenium/linux_release_x64"  
echo "====== BUILDING X64 RELEASE ======"
echo "running mkbundle on Netlenium Server (x64)"
mkbundle --simple --static --deps -v -o netlenium --config /etc/mono/config netlenium.exe
echo "deleting win32 compiled binaries"
rm netlenium.exe
echo "deleting leftover resources"
rm "ICSharpCode.SharpZipLib.xml"
rm "ICSharpCode.SharpZipLib.pdb"
rm "netlenium.exe.config"
rm "Newtonsoft.Json.xml"


echo "changing current directory"
cd "../linux_release_x86"
echo "====== BUILDING X86 RELEASE ======"
echo "running mkbundle on Netlenium Server (x86)"
mkbundle --simple --static --deps -v -o netlenium --config /etc/mono/config netlenium.exe
echo "deleting win32 compiled binaries"
rm netlenium.exe
echo "deleting leftover resources"
rm "ICSharpCode.SharpZipLib.xml"
rm "ICSharpCode.SharpZipLib.pdb"
rm "netlenium.exe.config"
rm "Newtonsoft.Json.xml"


echo "changing current directory"
cd "../linux_release_anycpu"
echo "====== BUILDING ARM RELEASE ======"
echo "running mkbundle on Netlenium Server (x86)"
mkbundle --simple --static --deps -v -o netlenium --config /etc/mono/config netlenium.exe
echo "deleting win32 compiled binaries"
rm netlenium.exe
echo "deleting leftover resources"
rm "ICSharpCode.SharpZipLib.xml"
rm "ICSharpCode.SharpZipLib.pdb"
rm "netlenium.exe.config"
rm "Newtonsoft.Json.xml"


echo "Done"

