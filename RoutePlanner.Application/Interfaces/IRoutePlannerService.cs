using RoutePlanner.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutePlanner.Application.Interfaces
{
    public interface IRoutePlannerService
    {
        RouteResult BuildRoute(List<Appointment> appointments, double? homeLat = null, double? homeLon = null);
    }
}
