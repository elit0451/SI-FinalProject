package com.lonelytravelers.rmiconnector;

import java.io.IOException;
import java.rmi.RemoteException;
import java.rmi.registry.Registry;
import java.util.logging.Level;
import java.util.logging.Logger;

public class Main {
    public static Registry registry;

    public Main() throws RemoteException {
    }

    public static void main(String[] args) {
        try {
            MessageGateway.ReceiveRequests();
        } catch (IOException ex) {
            Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
        }
        
        while(true){}
    }
}
