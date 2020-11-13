using System;
using System.Linq;

namespace Prague_Parking
{
    class Program
    {
        static void Main(string[] args)
        {
            //Init array with 100 empty parkingspaces
            string[] parkingSpaces = Enumerable.Repeat("EMPTY", 100).ToArray();
            bool running = true;
            while (running)
            {
                //Meny
                Console.Clear();
                Console.WriteLine("Menu: ");
                Console.WriteLine("1. Park car");
                Console.WriteLine("2. Park an MC");
                Console.WriteLine("3. Search for car");
                Console.WriteLine("4. Dispense vehicle");
                Console.WriteLine("5. Move vehicle");
                int menuChoice;
                Console.Write("Option: ");
                string input = Console.ReadLine();
                bool validMenuChoice = int.TryParse(input, out menuChoice);
                Console.WriteLine(validMenuChoice);
                Console.WriteLine(menuChoice);

                if (validMenuChoice)
                {
                    Console.Clear();
                    switch (menuChoice)
                    {
                        case 1:
                            Console.WriteLine("To park a car, enter the registration-info like: XXXXYYY (eg. XYZA123)");
                            string regInfo = Console.ReadLine();
                            if (regInfo.Length > 10)
                            {
                                Console.WriteLine("Maximum 10 characters");
                                Console.Read();
                            }
                            else
                            {
                                int parkingSpace = parkCar(parkingSpaces, regInfo);
                                Console.WriteLine("Park the car on space: {0}", parkingSpace);
                                Console.Read();
                            }
                            break;
                        case 2:
                            Console.WriteLine("To park a MC, enter the registration-info like: XXXXYYY (eg. XYZA123)");
                            string mcRegInfo = Console.ReadLine();
                            if (mcRegInfo.Length > 10)
                            {
                                Console.WriteLine("Maximum 10 characters");
                                Console.Read();
                            }
                            else
                            {
                                int mcParkingSpace = parkMc(parkingSpaces, mcRegInfo);
                                Console.WriteLine("Park the mc on space: {0}", mcParkingSpace);
                                Console.Read();
                            }
                            break;
                        case 3:
                            Console.WriteLine("Enter the registration-information of the vehicle: ");
                            string vehicleToSearchFor = Console.ReadLine();
                            int parkingSpotWithFoundVehicle = FindVehicle(parkingSpaces, vehicleToSearchFor);
                            if (parkingSpotWithFoundVehicle == -1)
                            {
                                Console.WriteLine("No vehicle found");
                            }
                            else
                            {
                                Console.WriteLine("Vehicle found on spot: {0}", parkingSpotWithFoundVehicle + 1);
                            }
                            Console.Read();
                            break;
                        case 4:
                            Console.WriteLine("To dispense a vehicle, enter the registration-info like: XXXXYYY (eg. XYZA123)");
                            string vehicleToDispense = Console.ReadLine();
                            string dispenseResult = DispenseVehicle(parkingSpaces, vehicleToDispense);
                            Console.WriteLine(dispenseResult);
                            Console.Read();
                            break;
                        case 5:
                            reparkVehicle(parkingSpaces);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        //Find first empty space in array
        public static int FindEmptySpace(string[] parkingLot)
        {
            int firstEmptyIndex = Array.FindIndex(parkingLot, row => row.Contains("EMPTY"));
            return firstEmptyIndex;
        }
        //Find a vehicle in array by registration info
        public static int FindVehicle(string[] parkingLot, string vehicleToSearchFor)
        {
            int parkingSpot = Array.FindIndex(parkingLot, row => row.Contains(vehicleToSearchFor));
            return parkingSpot;
        }
        //Find a parkingspot with a free spot for a mc
        public static int FindEmptySpaceForMc(string[] parkingLot)
        {
            int firstEmptyIndex = Array.FindIndex(parkingLot, row => row.Contains("FREE MC SPOT"));
            return firstEmptyIndex;
        }
        //Park a car
        public static int parkCar(string[] parkingLot, string regInfo, int specificParkingSpot = -1)
        {
            string carToPark = "CAR: " + regInfo;
            int emptyIndex;
            if (specificParkingSpot == -1)
            {
                emptyIndex = FindEmptySpace(parkingLot);
            }
            else
            {
                emptyIndex = specificParkingSpot;
            }

            parkingLot[emptyIndex] = carToPark;
            return emptyIndex + 1;
        }
        //Determine vehicletype my reginfo
        public static string DetermineVehicleType(string[] parkingLot, string vehicle)
        {
            int parkingSpot = Array.FindIndex(parkingLot, row => row.Contains(vehicle));
            string type;
            if (parkingSpot == -1)
            {
                type = "EMPTY";
            }
            else if (parkingLot[parkingSpot].Contains("CAR"))
            {
                type = "CAR";
            }
            else
            {
                type = "MC";
            }
            return type;
        }
        //Check if certain parkingspot has room for a vehicle based on vehicletype.
        public static bool CheckForFreeSpace(string[] parkingLot, string vehicleType, int position)
        {
            bool freeSpace;
            if (parkingLot[position].Contains("EMPTY"))
            {
                freeSpace = true;
            }
            else if (parkingLot[position].Contains("FREE MC SPOT") && vehicleType == "MC")
            {
                freeSpace = true;
            }
            else
            {
                freeSpace = false;
            }
            return freeSpace;
        }
        //Park motorcycle
        public static int parkMc(string[] parkingLot, string regInfo, int specificParkingSpot = -1)
        {
            string mcToPark = "MC: " + regInfo + ";";
            int emptyIndex;
            if (specificParkingSpot == -1)
            {
                emptyIndex = FindEmptySpaceForMc(parkingLot);
                if (emptyIndex == -1)
                {
                    emptyIndex = FindEmptySpace(parkingLot);
                    parkingLot[emptyIndex] = mcToPark + " " + "FREE MC SPOT";
                }
                else
                {
                    string[] splitted = parkingLot[emptyIndex].Split(";");
                    parkingLot[emptyIndex] = splitted[0] + ";" + mcToPark;
                }
                return emptyIndex + 1;
            }
            else
            {
                if (parkingLot[specificParkingSpot].Contains("EMPTY"))
                {
                    parkingLot[specificParkingSpot] = mcToPark + " " + "FREE MC SPOT";
                }
                else
                {
                    string[] splitted = parkingLot[specificParkingSpot].Split(";");
                    parkingLot[specificParkingSpot] = splitted[0] + ";" + mcToPark;
                }
                return specificParkingSpot;
            }
        }
        //Dispense a Vehicle
        public static string DispenseVehicle(string[] parkingLot, string vehicleToDispense)
        {
            int foundVehicleIndex = FindVehicle(parkingLot, vehicleToDispense);
            if (foundVehicleIndex == -1)
            {
                return "Can't find this vehicle";
            }
            else
            {
                if (parkingLot[foundVehicleIndex].Contains("CAR:"))
                {
                    parkingLot[foundVehicleIndex] = "EMPTY";
                    return $"Car with reg: {vehicleToDispense} dispensed from parkingspace {foundVehicleIndex + 1}.";
                }
                else if (parkingLot[foundVehicleIndex].Contains("FREE MC SPOT"))
                {
                    parkingLot[foundVehicleIndex] = "EMPTY";
                    return $"Mc with reg: {vehicleToDispense} dispensed from parkingspace {foundVehicleIndex + 1}.";
                }
                else
                {
                    string[] splitted = parkingLot[foundVehicleIndex].Split(";");
                    Console.WriteLine(splitted[0]);
                    Console.WriteLine(splitted[1]);
                    int indexOfVehicleToDispense = FindVehicle(splitted, vehicleToDispense);
                    if (indexOfVehicleToDispense == 0)
                    {
                        parkingLot[foundVehicleIndex] = splitted[1] + "; FREE MC SPOT";
                    }
                    else
                    {
                        parkingLot[foundVehicleIndex] = splitted[0] + "; FREE MC SPOT";
                    }
                    return $"Mc with reg: {vehicleToDispense} dispensed from parkingspace {foundVehicleIndex + 1}.";
                }
            }
        }
        //Repark/move a vehicle
        public static void reparkVehicle(string[] parkingLot)
        {
            Console.WriteLine("To repark a vehicle please enter the registration-info like: XXXXYYY (eg. XYZA123)");
            string vehicleToRepark = Console.ReadLine();
            string vehicleType = DetermineVehicleType(parkingLot, vehicleToRepark);
            if (vehicleType == "EMPTY")
            {
                Console.WriteLine("Vehicle {0} not found", vehicleToRepark);
                Console.Read();
            }
            else
            {

                Console.WriteLine("Where do you want to repark the vehicle? 1-100");
                int newParkingSpace = Int32.Parse(Console.ReadLine()) - 1;
                bool freeSpace = CheckForFreeSpace(parkingLot, vehicleType, newParkingSpace);
                if (!freeSpace)
                {
                    Console.WriteLine("No space for this vehicle {0} on parkingspace {1}, try another parkingspace.", vehicleToRepark, newParkingSpace + 1);
                    Console.Read();
                }
                else
                {
                    int oldParkingSpace = FindVehicle(parkingLot, vehicleToRepark);
                    if (vehicleType == "CAR")
                    {
                        Console.WriteLine(parkCar(parkingLot, vehicleToRepark, newParkingSpace));
                        DispenseVehicle(parkingLot, vehicleToRepark);
                        Console.WriteLine("{0} reparked on parkingspace {1} from parkingspace{2}.", vehicleType, newParkingSpace + 1, oldParkingSpace + 1);
                        Console.Read();
                    }
                    else
                    {
                        parkMc(parkingLot, vehicleToRepark, newParkingSpace);
                        DispenseVehicle(parkingLot, vehicleToRepark);
                        Console.WriteLine("{0} reparked on parkingspace {1} from parkingspace {2}.", vehicleType, newParkingSpace + 1, oldParkingSpace + 1);
                        Console.Read();

                    }
                }
            }
        }
    }
 }
