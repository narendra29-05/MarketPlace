using Autofac;
using Findly.Api.Auth;
using Findly.Application.Common.Interfaces;

namespace Findly.Api;

public class WebModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<LocalUser>()
            .As<ICurrentUser>()
            .SingleInstance();
    }
}
