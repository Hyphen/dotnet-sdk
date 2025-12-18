using Hyphen.Sdk;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHyphenEnv();

var host = builder.Build();

var env = host.Services.GetRequiredService<IEnv>();

Console.WriteLine("GLOBAL is '{0}', LOCAL is '{1}'", env.GetString("GLOBAL", required: true), env.GetString("LOCAL"));
Console.WriteLine("Half of OVERRIDE is {0}", env.GetInt("OVERRIDE") / 2);
