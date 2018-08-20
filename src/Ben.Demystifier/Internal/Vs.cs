using System.IO;
using System.Threading;

namespace System.Diagnostics.Internal
{
    class VS
    {
        internal class ThisAppDomain
        {
            private static int didSetup = 0;

            internal static void Setup()
            {
                if (Interlocked.CompareExchange(ref didSetup, 1, 0) == 0)
                {
                    AssemblyResolver.Load("Ben.Demystifier.VS.IPortablePdbReader.dll");
                    AssemblyResolver.Install();
                }
            }
        }

        internal class RemoteAppDomain
        {
            internal static AppDomain Setup()
            {
                var domain = AppDomain.CreateDomain("PortablePdbReaderDomain", securityInfo: null, info: new AppDomainSetup { ApplicationBase = Directory.GetCurrentDirectory() });
                domain.DoCallBack(new CrossAppDomainDelegate(() =>
                {
                    AssemblyResolver.Load("System.Collections.Immutable.dll");
                    AssemblyResolver.Load("System.Reflection.Metadata.dll");
                    AssemblyResolver.Load("Ben.Demystifier.VS.IPortablePdbReader.dll");
                    AssemblyResolver.Load("Ben.Demystifier.VS.PortablePdbReader.dll");
                }));
                domain.DoCallBack(new CrossAppDomainDelegate(AssemblyResolver.Install));
                return domain;
            }
        }
    }
}
