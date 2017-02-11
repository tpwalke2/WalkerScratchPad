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
        private SortedSet<int> _destinationFloors;

        public Elevator(string uid, int maxFloor)
        {
            this.UID = uid;
            _maxFloor = maxFloor;
            _currentFloor = 1;
            _currentStatus = ElevatorStatus.Idle;
            _observers = new List<IElevatorObserver>();
            _destinationFloors = new SortedSet<int>();
        }

        public void Tick()
        {
            // for each tick, this elevator may either change status, or change floors, but not both

            switch (_currentStatus)
            {
                case ElevatorStatus.Maintenance:
                    // elevator is in maintenance mode, do nothing
                    break;
                case ElevatorStatus.Idle:
                    if (_destinationFloors.Count > 0)
                    {
                        var nextDestination = _destinationFloors.ElementAt(0);
                        if (nextDestination > _currentFloor)
                        {
                            _currentStatus = ElevatorStatus.MovingUp;
                        } else if (nextDestination < _currentFloor)
                        {
                            _currentStatus = ElevatorStatus.MovingDown;
                        } else
                        {
                            // next destination is this floor
                            _currentStatus = ElevatorStatus.StoppedOpen;
                            _destinationFloors.Remove(nextDestination);
                            notifyOpened();
                        }
                    }
                    break;
            }
        }

        public bool AddDestination(int floorNum)
        {
            if ((floorNum > _maxFloor) || (floorNum < 1)) { return false; }
            return _destinationFloors.Add(floorNum);
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

        public void PerformMaintenance()
        {
            _currentStatus = ElevatorStatus.Idle;
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
