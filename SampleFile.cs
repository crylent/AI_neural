using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace AI_neural;

[DataContract]
[KnownType(typeof(SampleFile))]
[KnownType(typeof(BitImage8))]
public class SampleFile: IExtensibleDataObject
{
    private const string Dir = "samples";
    private const string Extension = "bitimg";
    
    [DataMember] public readonly string SampleName;
    [DataMember] public readonly BitImage8 Image;

    public SampleFile(string sampleName, BitImage8 image)
    {
        SampleName = sampleName;
        Image = image;
    }

    private static string CreateFilename(string sampleName) => $"{Dir}/{sampleName}.{Extension}";
    private string CreateFilename() => CreateFilename(SampleName);

    private static readonly DataContractSerializer Serializer = new(typeof(SampleFile));

    public void Save()
    {
        using var stream = File.Open(CreateFilename(), FileMode.Create);
        Serializer.WriteObject(stream, this);
    }

    private static SampleFile? Load(string file)
    {
        using var stream = File.Open(file, FileMode.Open);
        return Serializer.ReadObject(stream) as SampleFile;
    }

    private static IEnumerable<SampleFile> LoadAllSamples() =>
        from file in Directory.GetFiles(Dir)
        where file.EndsWith($".{Extension}") select Load(file) into sampleFile
        where sampleFile != null select sampleFile;

    public static IEnumerable<SampleFile> LoadAll()
    {
        Directory.CreateDirectory(Dir);
        return LoadAllSamples();
    }

    public ExtensionDataObject? ExtensionData { get; set; }
}