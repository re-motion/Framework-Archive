Using the ExtractSecurityMetadata task in your project
======================================================

Add the following lines to your project file ("<project name>.csproj").

<UsingTask TaskName="Rubicon.Security.MSBuild.ExtractSecurityMetadata"
           AssemblyFile="..\..\Security\MSBuild.Tasks\bin\Debug\Rubicon.Security.MSBuild.dll" />
           
<Target Name="AfterBuild">
  <ExtractSecurityMetadata Assemblies="$(TargetPath)" OutputFilename="$(TargetDir)security-metadata.xml" />
</Target>
