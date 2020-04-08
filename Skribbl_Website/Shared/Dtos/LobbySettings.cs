using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbySettings : IValidatableObject
    {
        public int RoundsLimit { get; set; } = 6;
        public List<int> PossibleRoundsLimit { get; private set; } = Enumerable.Range(1, 7).ToList();
        public int TimeLimit { get; set; } = 60;
        public List<int> PossibleTimeLimit { get; private set; } = Enumerable.Range(2, 5).Select(time => time * 15).ToList();
        public List<string> AdditionalWords { get; set; } = new List<string>();
        public bool UseOnlyAdditionalWords { get; set; } = false;

        public LobbySettings()
        {

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!PossibleRoundsLimit.Any(value => value == RoundsLimit))
            {
                yield return new ValidationResult("Select correct value.", new[] { nameof(RoundsLimit) });
            }
            if (!PossibleTimeLimit.Any(value => value == TimeLimit))
            {
                yield return new ValidationResult("Select correct value.", new[] { nameof(TimeLimit) });
            }
        }
    }
}
