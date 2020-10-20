import { AfterViewInit, Component } from '@angular/core';
import * as L from 'leaflet';
import { Router } from '@angular/router';
import { StandService } from '../../services/stand.service';

@Component({
  templateUrl: './market-map.component.html',
  styleUrls: ['./market-map.component.scss']
})
export class MarketMapComponent implements AfterViewInit {
  public map: any;
  public userPosMarker: any;
  public userCircleMarker: any;
  public showUserPos: boolean;

  constructor(
    private router: Router,
    private standService: StandService
  ) {
  }

  ngAfterViewInit(): void {
    this.map = L.map('map', {
      center: [39.8282, -98.5795],
      zoom: 3,
      zoomControl: false
    });

    const iconRetinaUrl = 'assets/marker-icon-2x.png';
    const iconUrl = 'assets/marker-icon.png';
    const shadowUrl = 'assets/marker-shadow.png';
    const iconDefault = L.icon({
      iconRetinaUrl,
      iconUrl,
      shadowUrl,
      iconSize: [25, 41],
      iconAnchor: [12, 41],
      popupAnchor: [1, -34],
      tooltipAnchor: [16, -28],
      shadowSize: [41, 41]
    });
    L.Marker.prototype.options.icon = iconDefault;

    const tiles = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    });

    this.map.setView(new L.LatLng(46.948316, 7.445910), 15);

    this.map.on('locationfound', this.onLocationFound.bind(this));

    tiles.addTo(this.map);

    this.standService.getAll().subscribe(stands => {
      for (let stand of stands) {
        L.marker([stand.latitude, stand.longitude]).addTo(this.map);
      }
    });
  }

  removeLocationMarkers() {
    if (this.userCircleMarker) {
      this.map.removeLayer(this.userCircleMarker);
      this.map.removeLayer(this.userPosMarker);
    }
  }

  onLocationFound(e) {
    if (this.showUserPos) {
      const radius = e.accuracy / 2;
      this.removeLocationMarkers();
      this.userPosMarker = L.marker(e.latlng);
      this.userPosMarker.addTo(this.map);
      this.userCircleMarker = L.circle(e.latlng, radius);
      this.userCircleMarker.addTo(this.map);
    }
  }

  locateMe() {
    this.showUserPos = !this.showUserPos;
    if (this.showUserPos) {
      if (this.userCircleMarker) {
        this.userPosMarker.addTo(this.map);
        this.userCircleMarker.addTo(this.map);
      }
      this.map.locate({watch: true});
    } else {
      this.removeLocationMarkers();
    }
  }

  openSettings() {
    this.router.navigate(['/stands']);
  }
}
