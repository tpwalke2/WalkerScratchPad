using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSim
{
    public class Elevator
    {
        enum ElevatorStatus
        {
            Idle,
            Stopping,
            StoppedOpen,
            StoppedClosed,
            MovingUp,
            MovingDown,
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
            _currentStatus = ElevatorStatus.Idle;
            _observers = new List<IElevatorObserver>();
        }

        public bool WillPass(int floorNum)
        {
            if ((floorNum > _maxFloor) || (floorNum < 1)) { return false; }
            if ((_currentStatus == ElevatorStatus.MovingUp) && (floorNum > _currentFloor)) { return true; }
            if ((_currentStatus == ElevatorStatus.MovingDown) && (floorNum < _currentFloor)) { return true; }
            return false;
        }

        public bool IsIdle()
        {
            return (_currentStatus == ElevatorStatus.Idle);
        }

        public string UID { get; }

        public void AddObserver(IElevatorObserver observer)
        {
            if (!_observers.Contains(observer)) { _observers.Add(observer); }
        }

        private void notifyClosed()
        {
            foreach (IElevatorObserver observer in _observers)
            {
                observer.ElevatorClosed(this);
            }
        }

        private void notifyOpened()
        {
            foreach (IElevatorObserver observer in _observers)
            {
                observer.ElevatorOpened(this);
            }
        }

        private void notifyChangedFloors(int currentFloor)
        {
            foreach (IElevatorObserver observer in _observers)
            {
                observer.ElevatorChangedFloors(this, currentFloor);
            }
        }
    }
}
