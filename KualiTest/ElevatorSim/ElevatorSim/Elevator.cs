﻿using System.Collections.Generic;
using System.Linq;

namespace ElevatorSim
{
    public class Elevator
    {
        enum ElevatorStatus
        {
            Idle,
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

        private int _floorsPassed;
        private int _tripsMade;

        public Elevator(string uid, int maxFloor)
        {
            this.UID = uid;
            _maxFloor = maxFloor;
            _currentFloor = 1;
            _currentStatus = ElevatorStatus.Idle;
            _observers = new List<IElevatorObserver>();
            _destinationFloors = new SortedSet<int>();

            _floorsPassed = 0;
            _tripsMade = 0;
        }

        /**
         * Tick this elevator given the current status, floor, and potential destinations.
         * 
         * A trip is counted as starting if currently in the idle state and the next destination starts a movement operation.
         */
        public void Tick()
        {
            switch (_currentStatus)
            {
                case ElevatorStatus.Maintenance:
                    // elevator is in maintenance mode, do nothing
                    break;
                case ElevatorStatus.Idle:
                    if (_tripsMade % 100 == 0)
                    {
                        // standard servicing cycle
                        _currentStatus = ElevatorStatus.Maintenance;
                        break;
                    }

                    // is there anything to do?
                    if (_destinationFloors.Count == 0) { break; }

                    if (_destinationFloors.ElementAt(0) > _currentFloor)
                    {
                        _currentStatus = ElevatorStatus.MovingUp;
                        _tripsMade++;
                    } else if (_destinationFloors.ElementAt(0) < _currentFloor)
                    {
                        _currentStatus = ElevatorStatus.MovingDown;
                        _tripsMade++;
                    } else
                    {
                        // next destination is this floor
                        _currentStatus = ElevatorStatus.StoppedOpen;
                        _destinationFloors.Remove(_destinationFloors.ElementAt(0));
                        notifyOpened();
                    }
                    break;
                case ElevatorStatus.MovingUp:
                    notifyChangedFloors(++_currentFloor);
                    _floorsPassed++;

                    if (_destinationFloors.ElementAt(0) == _currentFloor)
                    {
                        // next destination is this floor
                        _currentStatus = ElevatorStatus.StoppedOpen;
                        _destinationFloors.Remove(_destinationFloors.ElementAt(0));
                        notifyOpened();
                    } else if ((_destinationFloors.ElementAt(0) < _currentFloor) || (_currentFloor == _maxFloor))
                    {
                        _currentStatus = ElevatorStatus.MovingDown;
                    }

                    break;
                case ElevatorStatus.MovingDown:
                    notifyChangedFloors(--_currentFloor);
                    _floorsPassed++;

                    if (_destinationFloors.ElementAt(0) == _currentFloor)
                    {
                        // next destination is this floor
                        _currentStatus = ElevatorStatus.StoppedOpen;
                        _destinationFloors.Remove(_destinationFloors.ElementAt(0));
                        notifyOpened();
                    }
                    else if ((_destinationFloors.ElementAt(0) > _currentFloor) || (_currentFloor == 1))
                    {
                        _currentStatus = ElevatorStatus.MovingUp;
                    }

                    break;
                case ElevatorStatus.StoppedOpen:
                    _currentStatus = ElevatorStatus.StoppedClosed;
                    notifyClosed();
                    break;
                case ElevatorStatus.StoppedClosed:
                    if (_destinationFloors.Count == 0)
                    {
                        _currentStatus = ElevatorStatus.Idle;
                    } else if (_destinationFloors.ElementAt(0) > _currentFloor)
                    {
                        _currentStatus = ElevatorStatus.MovingUp;
                    }
                    else if (_destinationFloors.ElementAt(0) < _currentFloor)
                    {
                        _currentStatus = ElevatorStatus.MovingDown;
                    }
                    else
                    {
                        // next destination is this floor
                        _currentStatus = ElevatorStatus.StoppedOpen;
                        _destinationFloors.Remove(_destinationFloors.ElementAt(0));
                        notifyOpened();
                    }
                    break;
            }
        }

        /**
         * This is used by the bank to send an elevator to a floor. This should also be used from within the elevator to go to another floor.
         */
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

        public int TripsMade
        {
            get { return _tripsMade; }
        }

        public int FloorsPassed
        {
            get { return _floorsPassed; }
        }

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
