using Enterspeed.Cli.Domain.Models;
using MediatR;

namespace Enterspeed.Cli.Api.Domain;

public class UpdateDomainRequest : IRequest<UpdateDomainResponse[]>
{
    public DomainId DomainId { get; set; }
    public string Name { get; set; }
}

public class UpdateDomainResponse
{
    public DomainId Id { get; set; }

    public TenantId TenantId { get; set; }

    public string Name { get; set; }

    public string[] Hostnames { get; set; }
}