using HRAnalyticsPrescriptiveAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HRAnalytics.BL.Interfaces
{
    public interface ISolverBL
    {
        SolverResult SolveEquation(Input argInput);
    }
}
