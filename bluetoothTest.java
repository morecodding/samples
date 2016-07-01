package com.testing;

import android.Manifest;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.support.annotation.Nullable;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

public class MainActivity extends AppCompatActivity {

    private static final int REQUEST_ENABLE_BT = 1;
    private static final int PERMISSION_REQUEST_COARSE_LOCATION = 1;
    ConnectDeviceThread connThread;
    BluetoothAdapter mBluetoothAdapter;
    private List<String> mBluetoothDevicesDiscovered;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        initComponents();
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if(resultCode == RESULT_OK)
            ToastMessage("Seu bluetooth foi habilitado");
        else
            ToastMessage("Bluetooth nao habilitado");
    }


    private void initComponents(){
        mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
        mBluetoothDevicesDiscovered = new ArrayList<>();

        if (mBluetoothAdapter != null) {
            checkPermissions();
            enableBluetooth();
            startDiscovery();
        }else{
            ToastMessage("Seu dispositivo nao possui bluetooth");
        }
    }

    private void enableBluetooth(){
        if (!mBluetoothAdapter.isEnabled()) {
            Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
        }
    }

    private void startDiscovery(){
        if(mBluetoothAdapter.isDiscovering())
            mBluetoothAdapter.cancelDiscovery();

        if(mBluetoothAdapter.startDiscovery())
            ToastMessage("Buscando novos dispositivos...");

        Thread getDevices = new Thread(new Runnable() {
            @Override
            public void run() {
                try{
                    IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
                    registerReceiver(mBluetoothReceiver, filter);
                }catch(Exception ex){
                    MException.Log(ex);
                }
            }
        });

        getDevices.start();

    }


    private final BroadcastReceiver mBluetoothReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if(BluetoothDevice.ACTION_FOUND.equals(action)){
                BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
                mBluetoothDevicesDiscovered.add(device.getName() + "\n" + device.getAddress());
                if(device.getName().equals("deviceX")){
                    ToastMessage("BlackBox encontrado, vamos conectar...");
                    pairDevice(device);
                }
            }else if(action.equals("android.bluetooth.adapter.action.DISCOVERY_FINISHED")){
                if(mBluetoothDevicesDiscovered.size() == 0){

                    startDiscovery();
                }
            }
        }
    };

    private void ToastMessage(String message){
        Toast.makeText(MainActivity.this,message, Toast.LENGTH_SHORT).show();
    }

    private void checkPermissions(){
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            if (!ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission.ACCESS_COARSE_LOCATION)) {
                ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.ACCESS_COARSE_LOCATION, Manifest.permission.ACCESS_FINE_LOCATION}, PERMISSION_REQUEST_COARSE_LOCATION);
            }
        }
    }

    private void pairDevice(final BluetoothDevice device){
        try {
            new ConnectDeviceThread(device).start();
        }catch (Exception ex){
            MException.Log(ex);
        }
    }

    protected void onConnected(final BluetoothSocket socket){
        //TODO enviar um dado e resgatar um dado

        try {
            sendMessage(socket, "getDeviceId");
           String response = getResponseMessage(socket);
            MException.Log(response);
        } catch (Exception e) {
            e.printStackTrace();
            MException.Log(e);
        }
    }

    public void sendMessage(BluetoothSocket socket, String message){
        try {
            OutputStream mmOutputStream = socket.getOutputStream();
            mmOutputStream.write(message.getBytes());
        } catch (IOException e) {
            e.printStackTrace();
            MException.Log(e);
        }
    }

    public String getResponseMessage(BluetoothSocket socket) throws IOException {
        final InputStream mmInputStream;
        final byte delimiter = 33;
        int readBufferPosition = 0;
        String response = "";

        mmInputStream = socket.getInputStream();
        int bytesAvailable = mmInputStream.available();

        if(bytesAvailable > 0) {

            byte[] packetBytes = new byte[bytesAvailable];
            byte[] readBuffer = new byte[1024];
            mmInputStream.read(packetBytes);

            for (int i = 0; i < bytesAvailable; i++) {
                byte b = packetBytes[i];
                if (b == delimiter) {
                    byte[] encodedBytes = new byte[readBufferPosition];
                    System.arraycopy(readBuffer, 0, encodedBytes, 0, encodedBytes.length);
                    response = new String(encodedBytes, "US-ASCII");
                    break;
                } else {
                    readBuffer[readBufferPosition++] = b;
                }
            }
        }
        return response;
    }

    static class MException{
        static String TAG = "POC_ERROR";
        static void Log(Throwable ex){
            Log.d(TAG, ex.getLocalizedMessage(), ex);
        }
        static void Log(String msg){
            Log.d(TAG, msg);
        }
    }

    private class ConnectDeviceThread extends Thread{

        private BluetoothDevice device;
        private BluetoothSocket socket;
        private static final String guidApp = "94f39d29-7d6d-437d-973b-fba39e49d4ee";

        public ConnectDeviceThread(BluetoothDevice device){
            BluetoothSocket temp = null;
            device = device;

            try{
                temp = device.createInsecureRfcommSocketToServiceRecord(UUID.fromString(guidApp));
            }catch (IOException ex){
                MException.Log(ex);
            }
            socket = temp;
        }

        @Override
        public void run() {
            mBluetoothAdapter.cancelDiscovery();
            try{
                socket.connect();
                onConnected(socket);
              
            }
            catch (IOException ex){
                MException.Log(ex);
                cancel();
                return;
            }
        }

        public void cancel(){
            try{
                socket.close();
            }catch (IOException ex){
                MException.Log(ex);
            }
        }
    }

}
