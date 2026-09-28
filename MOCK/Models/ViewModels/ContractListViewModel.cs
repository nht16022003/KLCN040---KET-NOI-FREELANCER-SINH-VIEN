using System.Collections.Generic;

namespace MOCK.Models.ViewModels
{
    public class ContractListViewModel
    {
        public List<ContractDetailViewModel> Contracts { get; set; } = new();
        public int TotalContracts => Contracts.Count;
        public int ActiveContracts { get; set; }
        public int CompletedContracts { get; set; }
        public decimal TotalEscrowAmount { get; set; }

        public string? SelectedStatus { get; set; }
        public string? Keyword { get; set; }
    }
}
