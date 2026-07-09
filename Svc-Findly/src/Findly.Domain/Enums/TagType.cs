namespace Findly.Domain.Enums;

public enum TagType
{
    Integration = 1,   // connects to another tool, e.g. "Slack", "Salesforce"
    Technology = 2,    // built with / runs on, e.g. "React", "AWS-hosted"
    UseCase = 3,       // the job it does, e.g. "Ticketing", "Knowledge base"
    Platform = 4,      // where it runs, e.g. "iOS", "Android", "Web"
    General = 5,       // catch-all when nothing else fits

    Feature = 6,       // a standard capability, e.g. "Live chat", "SSO", "API access"
    Compliance = 7,    // security/legal certification, e.g. "SOC-2", "GDPR", "HIPAA"
    Deployment = 8,    // how it's hosted, e.g. "Cloud", "On-premise", "Self-hosted"
    Language = 9       // supported language, e.g. "English", "Spanish", "German"
}
