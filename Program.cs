using Topshelf;

namespace WindowsServiceApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<MessageProcess>(s =>
                {
                    s.ConstructUsing(messageProcess => new MessageProcess());
                    s.WhenStarted(messageProcess => messageProcess.Start());
                    s.WhenStopped(messageProcess => messageProcess.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("MessageProcess");
                x.SetDisplayName("Message Process Service");
                x.SetDescription("Process Message from RabbitMQ");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}