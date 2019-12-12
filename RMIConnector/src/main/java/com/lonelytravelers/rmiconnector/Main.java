/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.lonelytravelers.rmiconnector;

import DTOs.AvailabilityDetails;
import DTOs.CarDetails;
import interfaces.ICars;
import java.net.MalformedURLException;
import java.rmi.Naming;
import java.rmi.NotBoundException;
import java.rmi.RemoteException;
import java.rmi.registry.Registry;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Kast
 */
public class Main {
    public static Registry registry;

    public Main() throws RemoteException {
    }

    public static void main(String[] args) {
        try {
            ICars carClient = (ICars) Naming.lookup("//localhost/Cars");
            AvailabilityDetails availabilityDetails = new AvailabilityDetails(new SimpleDateFormat("yyyy-MM-dd").parse("2019-04-15"), new SimpleDateFormat("yyyy-MM-dd").parse("2019-04-20"), new DTOs.HotelDetails("24", "","",0,null,""));
            List<CarDetails> list = carClient.getAvailableCars(availabilityDetails);
            
            for(CarDetails carD : list)
            {
                System.out.println(carD.getLicensePlate());
            }
        } catch (NotBoundException | MalformedURLException | RemoteException | ParseException ex) {
            Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
