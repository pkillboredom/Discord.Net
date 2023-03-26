private async Task SendAsync(IAudioClient client, string path)
{
    // Create a cancellation token and handle forced disconnects by users.
    CancellationTokenSource ctxSource = new CancellationTokenSource();
    client.Disconnected += (ex =>
    {
        ctxSource.Cancel();
        return Task.CompletedTask;
    });
    // Create FFmpeg using the previous example
    using (var ffmpeg = CreateStream(path))
    using (var output = ffmpeg.StandardOutput.BaseStream)
    using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
    {
        try { await output.CopyToAsync(discord, ctxSource.Token); }
        finally { await discord.FlushAsync(ctxSource.Token); }
    }
}
