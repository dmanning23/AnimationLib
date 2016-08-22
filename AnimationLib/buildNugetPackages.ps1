rm *.nupkg
nuget pack .\AnimationLib.nuspec -IncludeReferencedProjects -Prop Configuration=Release
cp *.nupkg C:\Projects\Nugets\