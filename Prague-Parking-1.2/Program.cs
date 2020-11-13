using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Prague_Parking
{
    class Program
    {
        static void Main(string[] args)
        {
            //Init array with 100 empty parkingspaces
            string[] parkingSpaces = Enumerable.Repeat("EMPTY", 100).ToArray();
            //List to store optimizing report in
            List<string> latestOptimizingReport = new List<string>();
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
                Console.WriteLine("6. Show Parkinglot list");
                Console.WriteLine("7. Show graphic overview of the parkinglot");
                Console.WriteLine("8. Mc optimizing script");
                Console.WriteLine("9. Show latest optimizing report");
                Console.WriteLine("10. Generate Fake vehicles for testing");
                Console.WriteLine("11. Shutdown this system");
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
                            string regInfo = validateRegInfo(Console.ReadLine());
                            if (regInfo == "error")
                            {
                                Console.WriteLine("The registration info you've provided dont seem to follow czech standards");
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
                            string mcRegInfo = validateRegInfo(Console.ReadLine());
                            if(mcRegInfo == "error")
                            {
                                Console.WriteLine("The registration info you've provided dont seem to follow czech standards");
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
                            string vehicleToSearchFor = validateRegInfo(Console.ReadLine());
                            if (vehicleToSearchFor == "error")
                            {
                                Console.WriteLine("The registration info you've provided dont seem to follow czech standards");
                                Console.Read();
                            } else
                            {
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
                            }
                            break;
                        case 4:
                            Console.WriteLine("To dispense a vehicle, enter the registration-info like: XXXXYYY (eg. XYZA123)");
                            string vehicleToDispense = validateRegInfo(Console.ReadLine());
                            if(vehicleToDispense == "error")
                            {
                                Console.WriteLine("The registration info you've provided dont seem to follow czech standards");
                                Console.Read();
                            }
                            else
                            {
                                string dispenseResult = DispenseVehicle(parkingSpaces, vehicleToDispense);
                                Console.WriteLine(dispenseResult);
                                Console.Read();
                            }
                            break;
                        case 5:
                            reparkVehicle(parkingSpaces);
                            break;
                        case 6:
                            ShowParkingSpots(parkingSpaces);
                            Console.Read();
                            break;
                        case 7:
                            renderParkingOverview(parkingSpaces);
                            Console.Read();
                            break;
                        case 8:
                            Console.WriteLine("Running optimizing script...");
                            latestOptimizingReport = optimizeParkingLot(parkingSpaces);
                            Console.Read();
                            break;
                        case 9:
                            if(latestOptimizingReport.Count == 0)
                            {
                                Console.WriteLine("No old report to show..");
                                Console.Read();
                            } else
                            {
                                showLatestOptimizingReport(latestOptimizingReport);
                                Console.Read();
                            }
                            break;
                        case 10:
                            Console.WriteLine("Generate fake vehicles for testing");
                            Console.WriteLine("Enter the number of vehicles you want to generate: 1-99");
                            int amount;
                            bool parseSuccess = int.TryParse(Console.ReadLine(), out amount);
                            if (parseSuccess && amount < 100)
                            {
                                generateFakeVehicles(parkingSpaces, amount);
                                Console.Read();
                            }
                            else
                            {
                                Console.WriteLine("Please enter a valid number between 1-99.");
                                Console.Read();
                            }
                            break;
                        case 11:
                            Console.WriteLine("Are you sure you want to shutdown the system? yes/no");
                            string confirmAnswer = Console.ReadLine();
                            if(confirmAnswer == "yes")
                            {
                                Environment.Exit(0);
                            } else
                            {
                                Console.WriteLine("Not shutting down..");
                                Console.Read();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        //Validate users reginfo input
        public static string validateRegInfo(string regInfo)
        {
            if (regInfo.Length < 7)
            {
                Console.WriteLine("Your registration info should contain atleast 7 characters");
                return "error";
            }
            else if (regInfo.Length > 10)
            {
                Console.WriteLine("Your registration info should contain Maximum 10 characters");
                return "error";
            }
            else if (!Regex.IsMatch(regInfo, "^[a-zA-Z0-9À-ž]*$"))
            {
                Console.WriteLine("Your registration info should only contain letters and numbers");
                return "error";
            }
            else
            {
                return regInfo;
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
        //Check if certain parkingspot has room for a vehicle based on vehicletype
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
        //Show a list of parked vehicles with their registration-info
        public static void ShowParkingSpots(string[] parkingLot)
        {
            int emptySpaces = 0;
            int usedSpaces = 0;
            Console.WriteLine("List overview of the parkinglot: ");
            for (int i = 0; i < parkingLot.Length; i++)
            {
                if (parkingLot[i] != "EMPTY")
                {
                    Console.WriteLine(i + 1 + ": " + parkingLot[i]);
                    usedSpaces++;
                } else
                {
                    emptySpaces++;
                }
            }
            if (usedSpaces == 0)
            {
                Console.WriteLine("All the parkingspots are unused!");
            } else
            {
                Console.WriteLine("Used parkingspots: {0}", usedSpaces);
                Console.WriteLine("Free parkingspots: {0}", emptySpaces);
            }
        }
        //Render graphic overview of the parkinglot
        public static void renderParkingOverview(string[] parkingLot)
        {
            Console.WriteLine("Graphic overview:");
            Console.Write("Completely unused:");
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("    ");
            Console.ResetColor();
            Console.Write("  1 free mc spot:");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write("    ");
            Console.ResetColor();
            Console.Write("  Occupied by car:");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("    ");
            Console.ResetColor();
            Console.Write("  Occupied by 2 mc's:");
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("    ");
            Console.WriteLine("\n");
            for (int i = 0; i < parkingLot.Length; i = i + 4)
            {
                for (int x = 0; x < 4; x++)
                {
                    int tot = i + x;
                    string cellNmbr;
                    if (tot < 10)
                    {
                        cellNmbr = "00" + tot;
                    }
                    else if (tot < 100)
                    {
                        cellNmbr = "0" + tot;
                    }
                    else
                    {
                        cellNmbr = "" + tot;
                    }

                    var parkingCell = new System.Text.StringBuilder();
                    string cell;
                    if (parkingLot[tot].Contains("EMPTY"))
                    {
                        cell = "Parking-spot: " + cellNmbr + " - " + parkingLot[tot];
                        parkingCell.Append(String.Format("{0,-27}", cell));
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write(parkingCell);
                    }
                    else if (parkingLot[tot].Contains("FREE MC SPOT"))
                    {
                        cell = "Parking-spot: " + cellNmbr + " - " + "1 MC";
                        parkingCell.Append(String.Format("{0,-27}", cell));
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Write(parkingCell);
                    }
                    else
                    {
                        if (parkingLot[tot].Contains("CAR"))
                        {
                            cell = "Parking-spot: " + cellNmbr + " - " + "1 CAR";
                            parkingCell.Append(String.Format("{0,-27}", cell));
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.Write(parkingCell);
                        }
                        else
                        {
                            cell = "Parking-spot: " + cellNmbr + " - " + "2 MC";
                            parkingCell.Append(String.Format("{0,-27}", cell));
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.Write(parkingCell);
                        }
                    }
                }
                Console.WriteLine();

            };
            Console.ResetColor();
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
        public static List<String> optimizeParkingLot(string[] parkingLot)
        {
            List<string> report = new List<string>();
            List<parkingSpot> parkingSpotsToBeMerged = new List<parkingSpot>();
            for(int i = 0; i < parkingLot.Length; i++)
            {
                if(parkingLot[i].Contains("FREE MC SPOT"))
                {
                    string[] splitted = parkingLot[i].Split(";");
                    string regInfo = splitted[0];
                    parkingSpotsToBeMerged.Add(new parkingSpot() { indexInParkingLot = i, regInfo = regInfo });
                    Console.WriteLine("");
                }
            }
            if(parkingSpotsToBeMerged.Count < 2)
            {
                Console.WriteLine("No optimizing to be done.");
            } else
            {
                int amountToBeMerged = parkingSpotsToBeMerged.Count;
                while(amountToBeMerged > 1)
                {
                    string instruction = $"Repark {parkingSpotsToBeMerged.ElementAt(amountToBeMerged -1).regInfo} from parkingspot {parkingSpotsToBeMerged.ElementAt(amountToBeMerged - 1).indexInParkingLot +1} to parkingspot {parkingSpotsToBeMerged.ElementAt(amountToBeMerged - 2).indexInParkingLot +1}";
                    report.Add(instruction);
                    parkingSpotsToBeMerged.RemoveAt(amountToBeMerged - 1);
                    parkingSpotsToBeMerged.RemoveAt(amountToBeMerged - 2);
                    amountToBeMerged = amountToBeMerged - 2;
                    Console.WriteLine(instruction);
                }
            }
            return report;
        }
        //render a list of the latest optimizing report
        public static void showLatestOptimizingReport(List<string> oldReport)
        {
            Console.WriteLine("Latest report: ");
            for(int i = 0; i < oldReport.Count; i++)
            {
                Console.WriteLine(oldReport.ElementAt(i));
            }
        }
        //Generate fake vehicles, randomizes reg-info and generates ca. 1/3 mcs and 2/3 cars.
        public static void generateFakeVehicles(string[] parkingLot, int amount)
        {
            Random r = new Random();
            Console.WriteLine();
            for (int i = 0; i < amount; i++)
            {
                int fakeNmbrs = r.Next(100, 999);
                string fakeRegInfo = "FAKE" + fakeNmbrs;
                Console.Write(fakeRegInfo + " ");
                if (fakeNmbrs < 350)
                {
                    parkMc(parkingLot, fakeRegInfo);
                }
                else
                {
                    parkCar(parkingLot, fakeRegInfo);
                }
            }
            Console.WriteLine();
            Console.WriteLine("{0} vehicles was generated!", amount);
            Console.Read();
        }

    }
    //class for optimizingscript
    class parkingSpot
    {
        public string regInfo { get; set; }
        public int indexInParkingLot { get; set; }
    }
}
