using OpenNFP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared
{
    public class FakeCycleData
    {
        private static readonly FakeCycleData _instance;
        private static readonly ChartingRepo _repo;

        public DateTime CycleStartDate { get; set; } = DateTime.Today.AddDays(-10);
        static FakeCycleData()
        {
            _instance = new FakeCycleData();
            _repo = new ChartingRepo();
            init();
        }
        [Obsolete]
        public List<DayViewModel> Days { get; set; } = new()
        {
            new DayViewModel { Date = DateTime.Today.AddDays(-1), ClearBlueTest = 3, CervixOpenTest = 2},
            new DayViewModel { Date = DateTime.Today.AddDays(-2), ClearBlueTest = 3, CervixOpenTest = 1},
            new DayViewModel { Date = DateTime.Today.AddDays(-3), ClearBlueTest = 2, CervixOpenTest = 1},
            new DayViewModel { Date = DateTime.Today.AddDays(-4), ClearBlueTest = 2, CervixOpenTest = 1},
            new DayViewModel { Date = DateTime.Today.AddDays(-5), ClearBlueTest = 1, CervixOpenTest = 1},
            new DayViewModel { Date = DateTime.Today.AddDays(-6), ClearBlueTest = 1, Coitus = true, CervixOpenTest = 1},
            new DayViewModel { Date = DateTime.Today.AddDays(-7), ClearBlueTest = 1, BleedingAmount = 1, CervixOpenTest = 2 },
            new DayViewModel { Date = DateTime.Today.AddDays(-8), ClearBlueTest = 0, BleedingAmount = 2, CervixOpenTest = 2 },
            new DayViewModel { Date = DateTime.Today.AddDays(-9), ClearBlueTest = 1, BleedingAmount = 3, CervixOpenTest = 3 },
            new DayViewModel { Date = DateTime.Today.AddDays(-10), ClearBlueTest = 1, Coitus = true, BleedingAmount = 1, CervixOpenTest = 1 },
        };

        [Obsolete]
        public static FakeCycleData Instance => _instance;
        public static ChartingRepo Repo => _repo;

        private static void init()
        {
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Open } );
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Closed } );
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed } );
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed } );
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-5), ClearBlueResult = ClearBlueResult.Low, CervixOpening = CervixOpening.Closed } );
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-6), ClearBlueResult = ClearBlueResult.Low, Coitus = true, CervixOpening = CervixOpening.Closed } );
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-7), ClearBlueResult = ClearBlueResult.Low, BleedingAmount = Flow.Spotting, CervixOpening = CervixOpening.Partial } );
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-8), ClearBlueResult = ClearBlueResult.Unknown, BleedingAmount = Flow.Light, CervixOpening = CervixOpening.Partial } );
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-9), ClearBlueResult = ClearBlueResult.Low, BleedingAmount = Flow.Heavy, CervixOpening = CervixOpening.Open } );
            _repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), ClearBlueResult = ClearBlueResult.Low, Coitus = true, BleedingAmount = Flow.Spotting, CervixOpening = CervixOpening.Closed });
        }
    }
}
