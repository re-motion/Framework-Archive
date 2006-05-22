Using the ExtractSecurityMetadata task in your project
======================================================

Add the following lines to your project file ("<project name>.csproj").

<UsingTask TaskName="Rubicon.Security.MSBuild.Tasks.ExtractSecurityMetadata" 
           AssemblyFile="..\..\Security\MSBuild.Tasks\bin\Debug\Rubicon.Security.MSBuild.Tasks.dll" />
           
<Target Name="AfterBuild">
  <ExtractSecurityMetadata Assemblies="$(TargetPath)" OutputFilename="$(TargetDir)security-metadata.xml" />
</Target>
