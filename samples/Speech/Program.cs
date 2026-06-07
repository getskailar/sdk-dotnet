using Skailar;

using var client = new SkailarClient();

await using Stream audio = await client.Audio.Speech.CreateAsync(new SpeechRequest
{
    Input = "Hello from Skailar. This audio was synthesized through the gateway.",
    Voice = SkailarVoice.Nova,
});

const string path = "speech.mp3";
await using (FileStream file = File.Create(path))
{
    await audio.CopyToAsync(file);
}

Console.WriteLine($"Wrote {new FileInfo(path).Length} bytes to {path}");
