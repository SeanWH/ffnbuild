using Serilog;

string?[] _testAddresses = [
    "https://archiveofourown.org/users/HowlingGuardian/pseuds/HowlingGuardian",
    "https://archiveofourown.org/works/30633947",
    "//www.fanfiction.net/s/6964817/1/Chosen-Ones",
    "https://www.fanfiction.net/s/6964817/1/Chosen-Ones",
    "/u/2139446/arturus",
    "https://www.fanfiction.net/u/2139446/arturus",
    "https://www.fanfiction.net/community/The-Archive-for-Self-Inserts-and-Original-Characters/117072/",
    "https://www.yahoo.com",
    "",
    null
];

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("ParsingResults-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information("----------------------------------------------\\nParsing Test Addresses");

foreach( string? address in _testAddresses )
{
    if( string.IsNullOrEmpty(address) )
    {
        if( address is null )
        {
            Log.Information("Address is NULL");
            continue;
        }
        else
        {
            Log.Information("Empty Address");
            continue;
        }
    }

    if( address.StartsWith("https://archiveofourown.org") )
    {
        ParseA03Address(address);
    }
    else
    {
        ParseFfnAddress(address);
    }
}

static void ParseFfnAddress(string address)
{
    Log.Information("Fanfiction.net Address: {Address}", address);
    var parts = SplitAddress(address);
}

static void ParseA03Address(string address)
{
    Log.Information("A03 Address: {Address}", address);
    var parts = SplitAddress(address);
}

static string NormalizeAddress(string address)
{
    if( address.StartsWith("https:") )
    {
        address = address.Replace("https:", string.Empty);
    }

    if( address.StartsWith("http:") )
    {
        address = address.Replace("http:", string.Empty);
    }

    if( address.StartsWith("//www.fanfiction.net") )
    {
        address = address.Replace("//www.fanfiction.net/", string.Empty);
    }

    if( address.StartsWith("//www.fanfiction.net") )
    {
        address = address.Replace("//www.fanfiction.net/", string.Empty);
    }

    if( address.StartsWith("//archiveofourown.org") )
    {
        address = address.Replace("//archiveofourown.org/", string.Empty);
    }

    return address;
}

static string[] SplitAddress(string address)
{
    Log.Information("Splitting Address: {Address}", address);
    address = NormalizeAddress(address);
    Log.Information("Protocol Removed: {Address}", address);

    var parts = address.Split('/', StringSplitOptions.RemoveEmptyEntries);
    Log.Information("Parts: {Parts}", parts);
    Log.Information("Count: {Count}", parts.Length);

    return parts;
}

await Log.CloseAndFlushAsync();