using Autofac;
using Findly.Application.Common.Interfaces;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence;
using Findly.Infrastructure.Security;

namespace Findly.Infrastructure;

public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DatabaseContext>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(typeof(InfrastructureModule).Assembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();

        builder.RegisterType<PasswordHasher>()
            .As<IPasswordHasher>()
            .SingleInstance();

        builder.RegisterType<JwtTokenGenerator>()
            .As<IJwtTokenGenerator>()
            .SingleInstance();
    }
}
