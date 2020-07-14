using System.ComponentModel;

namespace Cashback.Enum
{
    public enum EnumStatus
    {
        [Description("Em Validação")]
        OnChecking,

        [Description("Aprovado")]
        Approved,

        [Description("Reprovado")]
        Unapproved
    }    
}
