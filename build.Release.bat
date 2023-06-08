msbuild Sedulous.proj /t:BuildAndroid /property:Configuration=Release
msbuild Sedulous.proj /t:BuildNETCore /property:Configuration=Release
msbuild Sedulous.proj /t:BuildWindowsForms /property:Configuration=Release
msbuild Sedulous.proj /t:BuildNETCoreTools /property:Configuration=Release
msbuild Sedulous.proj /t:CopyNETCoreBinaries /property:Configuration=Release
msbuild Sedulous.proj /t:CopyAndroidBinaries /property:Configuration=Release
msbuild Sedulous.proj /t:CopyWindowsFormsBinaries /property:Configuration=Release