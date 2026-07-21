using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.UnitTests.Domain;

public class LeadTests
{
    private static Lead NewLead(string? phone = null) =>
        Lead.Create(1, 1, LeadType.RequestDemo, "Jane CTO", "jane@bigco.com", phone, "BigCo", 500, "Demo please", "system");

    [Fact]
    public void Create_starts_new()
    {
        var lead = NewLead();

        Assert.Equal(LeadStatus.New, lead.Status);
        Assert.Equal("jane@bigco.com", lead.BusinessEmail.Value);
        Assert.Null(lead.Phone);
    }

    [Fact]
    public void Create_with_phone_keeps_it()
    {
        var lead = NewLead("+14155550101");

        Assert.NotNull(lead.Phone);
        Assert.Equal("+14155550101", lead.Phone!.Value);
    }

    [Theory]
    [InlineData(LeadStatus.Contacted)]
    [InlineData(LeadStatus.Lost)]
    public void New_can_go_to_contacted_or_lost(LeadStatus target)
    {
        var lead = NewLead();

        lead.TransitionTo(target, "vendor");

        Assert.Equal(target, lead.Status);
    }

    [Theory]
    [InlineData(LeadStatus.Qualified)]
    [InlineData(LeadStatus.Converted)]
    [InlineData(LeadStatus.New)]
    public void New_cannot_skip_stages(LeadStatus target)
    {
        var lead = NewLead();

        Assert.Throws<InvalidOperationException>(() => lead.TransitionTo(target, "vendor"));
    }

    [Fact]
    public void Full_happy_path_new_contacted_qualified_converted()
    {
        var lead = NewLead();

        lead.TransitionTo(LeadStatus.Contacted, "v");
        lead.TransitionTo(LeadStatus.Qualified, "v");
        lead.TransitionTo(LeadStatus.Converted, "v");

        Assert.Equal(LeadStatus.Converted, lead.Status);
    }

    [Fact]
    public void Contacted_cannot_jump_to_converted()
    {
        var lead = NewLead();
        lead.TransitionTo(LeadStatus.Contacted, "v");

        Assert.Throws<InvalidOperationException>(() => lead.TransitionTo(LeadStatus.Converted, "v"));
    }

    [Theory]
    [InlineData(LeadStatus.Converted)]
    [InlineData(LeadStatus.Lost)]
    public void Terminal_states_allow_no_transitions(LeadStatus terminal)
    {
        var lead = NewLead();
        if (terminal == LeadStatus.Converted)
        {
            lead.TransitionTo(LeadStatus.Contacted, "v");
            lead.TransitionTo(LeadStatus.Qualified, "v");
            lead.TransitionTo(LeadStatus.Converted, "v");
        }
        else
        {
            lead.TransitionTo(LeadStatus.Lost, "v");
        }

        Assert.Throws<InvalidOperationException>(() => lead.TransitionTo(LeadStatus.Contacted, "v"));
    }
}
