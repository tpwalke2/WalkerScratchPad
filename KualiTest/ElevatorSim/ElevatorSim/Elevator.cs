using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSim
{
    public class Elevator: IElevatorObserver
    {
        enum ElevatorStatus
        {
            StoppedOpen,
            StoppedClosed,
            Moving,
            Maintenance
        }

        private int _maxFloor;
        private ElevatorStatus _currentStatus;
        private int _currentFloor;
        private IList<IElevatorObserver> _observers;

        public Elevator(string uid, int maxFloor)
        {
            this.UID = uid;
            _maxFloor = maxFloor;
            _currentFloor = 1;
            _currentStatus = ElevatorStatus.StoppedClosed;
            _observers = new List<IElevatorObserver>();
        }

        public string UID { get; }

        public void AddObserver(IElevatorObserver observer)
        {
            if (!_observers.Contains(observer)) { _observers.Add(observer); }
        }
    }
}
