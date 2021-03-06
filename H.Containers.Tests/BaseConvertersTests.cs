using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Containers.Tests.Utilities;
using H.NET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Containers.Tests
{
    public static class BaseConvertersTests
    {
        public static async Task StartStreamingRecognitionTest(IConverter converter, string name, string expected, int bytesPerWrite = 8000)
        {
            using var recognition = await converter.StartStreamingRecognitionAsync();
            recognition.AfterPartialResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterPartialResults: {args.Text}");
            recognition.AfterFinalResults += (sender, args) =>
            {
                Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterFinalResults: {args.Text}");

                Assert.AreEqual(expected, args.Text);
            };

            var bytes = ResourcesUtilities.ReadFileAsBytes(name);
            // 44 - is default wav header length
            for (var i = 44; i < bytes.Length; i += bytesPerWrite)
            {
                var chunk = new ArraySegment<byte>(bytes, i, i < bytes.Length - bytesPerWrite ? bytesPerWrite : bytes.Length - i).ToArray();
                await recognition.WriteAsync(chunk);

                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }

            await recognition.StopAsync();
        }

        public static async Task StartStreamingRecognitionTest_RealTime(IRecorder recorder, IConverter converter, bool writeWavHeader = false)
        {
            await recorder.InitializeAsync();
            await recorder.StartAsync();

            using var recognition = await converter.StartStreamingRecognitionAsync();
            recognition.AfterPartialResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterPartialResults: {args.Text}");
            recognition.AfterFinalResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterFinalResults: {args.Text}");

            if (writeWavHeader)
            {
                var wavHeader = recorder.WavHeader?.ToArray() ??
                                     throw new InvalidOperationException("Recorder Wav Header is null");
                await recognition.WriteAsync(wavHeader);
            }
            if (recorder.RawData != null)
            {
                await recognition.WriteAsync(recorder.RawData.ToArray());
            }

            // ReSharper disable once AccessToDisposedClosure
            recorder.RawDataReceived += async (sender, args) => await recognition.WriteAsync(args.RawData.ToArray());

            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            await recorder.StopAsync();
            await recognition.StopAsync();
        }

        public static async Task ConvertTest(IConverter converter, string name, string expected, CancellationToken cancellationToken = default)
        {
            var bytes = ResourcesUtilities.ReadFileAsBytes(name);
            var actual = await converter.ConvertAsync(bytes, cancellationToken);

            Assert.AreEqual(expected, actual);
        }

        public static async Task ConvertTest_RealTime(IRecorder recorder, IConverter converter)
        {
            await recorder.InitializeAsync();
            await recorder.StartAsync();

            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            await recorder.StopAsync();

            var bytes = recorder.WavData;

            var result = await converter.ConvertAsync(bytes.ToArray());

            Console.WriteLine(result);
        }
    }
}
