import {AfterViewInit, Component} from '@angular/core';
import * as L from 'leaflet';

@Component({
  templateUrl: './market-map.component.html',
  styleUrls: ['./market-map.component.scss']
})
export class MarketMapComponent implements AfterViewInit {
  private map: any;

  ngAfterViewInit(): void {
    this.map = L.map('map', {
      center: [ 39.8282, -98.5795 ],
      zoom: 3,
      zoomControl: false
    });

    const tiles = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    });

    this.map.setView(new L.LatLng(46.948316, 7.445910), 15);

    const onLocationFound = (e) => {
      const radius = e.accuracy / 2;
      L.marker(e.latlng).addTo(this.map);
      L.circle(e.latlng, radius).addTo(this.map);
    };

    this.map.on('locationfound', onLocationFound);
    this.map.locate({ watch: true });

    tiles.addTo(this.map);
  }
}
