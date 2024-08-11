import React, { useEffect } from 'react';
import { useMap } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet-routing-machine';
import 'leaflet-routing-machine/dist/leaflet-routing-machine.css';

function RoutingMachine({ userPosition, branchPosition, onRouteCalculated }) {
  const map = useMap();

  useEffect(() => {
    if (!map) return;

    const routingControl = L.Routing.control({
      waypoints: [L.latLng(userPosition), L.latLng(branchPosition)],
      lineOptions: {
        styles: [
          {
            color: "blue",
            weight: 6,
            opacity: 0.7,
          },
        ],
      },
      routeWhileDragging: false,
      geocoder: L.Control.Geocoder.nominatim(),
      addWaypoints: false,
      fitSelectedRoutes: true,
      showAlternatives: true,
      draggableWaypoints: false,
      createMarker: function () { return null; } // Prevent automatic marker creation
    }).addTo(map);

    routingControl.on('routesfound', function (e) {
      const routes = e.routes;
      const summary = routes[0].summary;
      const distance = summary.totalDistance / 1000; // Distance in meters
      const time = summary.totalTime; // Time in seconds

      // Call the callback function with distance and time
      if (onRouteCalculated) {
        onRouteCalculated(distance, time);
      }
    });

    // Cleanup function to remove routing control on component unmount
    return () => {
      if (routingControl) {
        routingControl.getPlan().setWaypoints([]);
        map.removeControl(routingControl);
      }
    };
  }, [map, userPosition, branchPosition, onRouteCalculated]);

  return null;
}

export default RoutingMachine;
