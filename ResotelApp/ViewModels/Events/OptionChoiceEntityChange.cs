using ResotelApp.ViewModels.Entities;

namespace ResotelApp.ViewModels.Events
{
    class OptionChoiceEntityChange
    {
        public OptionChangeKind Kind { get; set; }
        public OptionChoiceEntity OptionChoiceEntity { get; set; }

        public OptionChoiceEntityChange(OptionChangeKind kind, OptionChoiceEntity optChoiceEntity)
        {
            Kind = kind;
            OptionChoiceEntity = optChoiceEntity;
        }

    }
}
