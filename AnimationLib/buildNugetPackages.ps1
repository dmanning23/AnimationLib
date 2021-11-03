rm *.nupkg
nuget pack .\AnimationLib.nuspec -IncludeReferencedProjects -Prop Configuration=Release
nuget pack .\AnimationLib.Bridge.nuspec -IncludeReferencedProjects -Prop Configuration=Release
cp *.nupkg C:\Projects\Nugets\