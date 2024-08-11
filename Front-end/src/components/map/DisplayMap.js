import React, { useEffect, useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import L from 'leaflet';
import "leaflet-control-geocoder/dist/Control.Geocoder.css";
import "leaflet-control-geocoder/dist/Control.Geocoder.js";
import RoutingMachine from './RoutingMachine';
import { getGeocodeFromAddress } from './GeocoderLocation';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

function DisplayMap({ address, address2, onDistanceCalculated }) {
  const [userPosition, setUserPosition] = useState(null);
  const [destination, setDestination] = useState([10.765268969704836, 106.60137392529515]);

  useEffect(() => {
    if (address) {
      getGeocodeFromAddress(address)
        .then(result => {
          if (result) {
            console.log("Destination geocode:", result);
            setDestination(result);
          }
        })
        .catch(error => console.error(error));
    }
  }, [address]);

  useEffect(() => {
    if (address2) {
      getGeocodeFromAddress(address2)
        .then(result => {
          if (result) {
            console.log("User geocode:", result);
            setUserPosition(result);
          }
        })
        .catch(error => console.error(error));
    }
  }, [address2]);

  const defaultCenter = [10.875376656860935, 106.80076631184579];

  return (
    <MapContainer center={destination || defaultCenter} zoom={13} scrollWheelZoom={false}>
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      {userPosition && (
        <Marker position={userPosition} icon={originIcon}>
          <Popup>Your Location</Popup>
        </Marker>
      )}
      {destination && (
        <Marker position={destination} icon={destinationIcon}>
          <Popup>Branch Location</Popup>
        </Marker>
      )}
      {destination && <UpdateMapView position={destination || defaultCenter} />}
      {userPosition && destination && (
        <RoutingMachine
          userPosition={userPosition}
          branchPosition={destination}
          onRouteCalculated={(distance, time) => onDistanceCalculated(distance)}
        />
      )}
    </MapContainer>
  );
}

function UpdateMapView({ position }) {
  const map = useMap();
  if (position) {
    map.flyTo(position, 14);
  }
  return null;
}

const destinationIcon = L.icon({
  iconUrl: 'https://cdn0.iconfinder.com/data/icons/small-n-flat/24/678111-map-marker-512.png',
  iconSize: [35, 35],
  iconAnchor: [17, 35],
  popupAnchor: [0, -35]
});

const originIcon = L.icon({
  iconUrl: 'https://cdn3.iconfinder.com/data/icons/map-14/144/Map-10-512.png',
  iconSize: [60, 60],
  iconAnchor: [17, 35],
  popupAnchor: [0, -35]
});

export default DisplayMap;
