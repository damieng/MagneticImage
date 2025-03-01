namespace MagneticImage.Sandbox;

class Arguments
{
    public string InputFile { get; }

    public bool IsValid
        => !string.IsNullOrWhiteSpace(InputFile);

    public Arguments(string[] args)
    {
        if (args.Length > 0)
            InputFile = args[0];
    }
}