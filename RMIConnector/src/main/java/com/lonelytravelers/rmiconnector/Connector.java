package com.lonelytravelers.rmiconnector;

import DTOs.AvailabilityDetails;
import DTOs.CarDetails;
import DTOs.CarTypeDetails;
import DTOs.DriverDetails;
import DTOs.HotelDetails;
import DTOs.NewBookingDetails;
import interfaces.IBooking;
import interfaces.ICars;
import java.net.MalformedURLException;
import java.rmi.Naming;
import java.rmi.NotBoundException;
import java.rmi.RemoteException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.json.JSONArray;
import org.json.JSONObject;

public class Connector {
    public static void addNewBooking(String pickUpTime, String deliveryTime, String pickUpPlace, String deliveryPlace, String driverId, String carId)
    {
        try {
            /*SETUP VARS FOR NEW BOOKING*/
            NewBookingDetails newBookingDetails = new NewBookingDetails();
            CarTypeDetails carDetails = new CarTypeDetails("", "", 0, 0);
            CarDetails car = new CarDetails(carId, carDetails);
            DriverDetails driver = new DriverDetails(driverId, "", new SimpleDateFormat("yyyy-MM-dd").parse("1966-10-18"), "22-9484786");
            
            newBookingDetails.setPickUpTime(new SimpleDateFormat("yyyy-MM-dd").parse(pickUpTime));
            newBookingDetails.setDeliveryTime(new SimpleDateFormat("yyyy-MM-dd").parse(deliveryTime));
            newBookingDetails.setPickUpPlace(new HotelDetails(pickUpPlace, "","",0,null,""));
            newBookingDetails.setDeliveryPlace(new HotelDetails(deliveryPlace, "","",0,null,""));
            newBookingDetails.setDriver(driver);
            newBookingDetails.setCar(car);
            
            
            /*CONNECTING BOOKING CLIENT RMI*/
            IBooking bookingClient = (IBooking) Naming.lookup("rmi://legacy:1099/Booking");
            
            
            /*CREATE ADD BOOKING REQUEST AND PROCESSING*/
            bookingClient.addNewBooking(newBookingDetails);
            
        } catch (ParseException | NotBoundException | MalformedURLException | RemoteException ex) {
            Logger.getLogger(Connector.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
    public static String getAvailableCars(String from, String to, String stationId)
    {
        
        JSONArray jsonArr = new JSONArray();
        try {
            /*SETUP VARS FOR AVAILABL CARS*/
            AvailabilityDetails availabilityDetails = new AvailabilityDetails(
                    new SimpleDateFormat("yyyy-MM-dd").parse(from), 
                    new SimpleDateFormat("yyyy-MM-dd").parse(to), 
                    new HotelDetails(stationId, "","",0,null,""));

            
            /*CONNECTING CAR CLIENT RMI*/
            ICars carClient = (ICars) Naming.lookup("rmi://legacy:1099/Cars");
            
            /*CREATE AND PROCESS CAR DATA*/
            List<CarDetails> list = carClient.getAvailableCars(availabilityDetails);
            
            
            for(CarDetails carD : list)
            {
                JSONObject jsonCar = new JSONObject();
                jsonCar.put("License", carD.getLicensePlate());
                jsonCar.put("Type", carD.getCarType().getName());
                jsonCar.put("Seats", carD.getCarType().getNumberOfSeats());
                jsonCar.put("Price", carD.getCarType().getPricePerDay());
                jsonArr.put(jsonCar);
            }            
        } catch (NotBoundException | MalformedURLException | RemoteException | ParseException  ex) {
            System.out.println("GOT EXCEPTION: " + ex.getMessage());
            Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
        }
        
        return jsonArr.toString();
    }
}
