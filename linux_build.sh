echo "restoring nuget packages"
nuget restore "Netlenium/Netlenium.sln"

echo "deleting old builds"
rm -rf "Build/Netlenium/Server/release_linux_x64"
rm -rf "Build/Netlenium/Server/release_linux_x86"
rm -rf "Build/Netlenium/Server/debug_linux_x64"
rm -rf "Build/Netlenium/Server/debug_linux_x86"

echo "building x64 release using msbuild"
msbuild /p:Configuration="Linux Release x64" "Netlenium/Netlenium.sln" -t:build

echo "building x86 release using msbuild"
msbuild /p:Configuration="Linux Release x86" "Netlenium/Netlenium.sln" -t:build

echo "building x64 debug using msbuild"
msbuild /p:Configuration="Linux Debug x64" "Netlenium/Netlenium.sln" -t:build

echo "building x86 debug using msbuild"
msbuild /p:Configuration="Linux Debug x86" "Netlenium/Netlenium.sln" -t:build

echo "changing current directory"
cd "Build/NetleniumServer/release_linux_x64"  

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
cd "../release_linux_x86"

echo "running mkbundle on Netlenium Server (x86)"
mkbundle --simple --static --deps -v -o netlenium --config /etc/mono/config netlenium.exe

echo "deleting win32 compiled binaries"
rm netlenium.exe

echo "deleting leftover resources"
rm "ICSharpCode.SharpZipLib.xml"
rm "ICSharpCode.SharpZipLib.pdb"
rm "netlenium.exe.config"
rm "Newtonsoft.Json.xml"

echo "done"
