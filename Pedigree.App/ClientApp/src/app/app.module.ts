import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FlexLayoutModule, StyleUtils } from "@angular/flex-layout";
import { SatPopoverModule } from '@ncstate/sat-popover';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatStepperModule } from '@angular/material';

/* Angular material */
import { AngularMaterialModule } from './angular-material.module';

import { AppComponent } from './app.component';
import { NavService } from './nav.service';
import { ChartService } from './chart.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SideNavbarComponent } from './_layouts/side-navbar/side-navbar.component';
import { TopNavbarComponent } from './_layouts/top-navbar/top-navbar.component';
import { MenuListItemComponent } from './_layouts/menu-list-item/menu-list-item.component';
import { HorseListComponent } from './components/horses/horse-list/horse-list.component';
import { HorseFormComponent } from './components/horses/horse-form/horse-form.component';
import { AddHorseComponent } from './components/horses/add-horse/add-horse.component';
import { AddHorsesComponent } from './components/horses/add-horses/add-horses.component';
import { EditHorseComponent } from './components/horses/edit-horse/edit-horse.component';
import { HorseFormExComponent } from './components/horses/horse-form-ex/horse-form-ex.component';
import { HorseService } from './_services/horse.service';
import { RaceService } from './_services/race.service';
import { ConfirmComponent } from './_shared/confirm/confirm.component';
import { AlertComponent } from './_shared/alert/alert.component';
import { NotificationSnackComponent } from './_shared/notification-snack/notification-snack.component';
import { HorseSearchComponent } from './_shared/horse-search/horse-search.component';
import { HeirarchyTreeComponent } from './components/charts/heirarchy-tree/heirarchy-tree.component';
import { LineageTreeComponent } from './components/charts/lineage-tree/lineage-tree.component';
import { HierarchyViewComponent } from './components/charts/hierarchy-view/hierarchy-view.component';
import { LineageGraphComponent } from './components/charts/lineage-graph/lineage-graph.component';
import { LineageGraphDialogComponent } from './components/charts/lineage-graph/lineage-graph-dialog.component';
import { RadialLineageGraphComponent } from './components/charts/radial-lineage-graph/radial-lineage-graph.component';
import { LinebreedingListComponent } from './components/calculates/linebreeding-list/linebreeding-list.component';
import { EquivalentsListComponent } from './components/calculates/equivalents-list/equivalents-list.component';
import { HypotheticalHierarchyViewComponent } from './components/charts/hypothetical-hierarchy-view/hypothetical-hierarchy-view.component';
import { HorseListViewComponent } from './_shared/horse-list-view/horse-list-view.component';
import { RaceListComponent } from './components/races/race-list/race-list.component';
import { AddRaceComponent } from './components/races/add-race/add-race.component';
import { EditRaceComponent } from './components/races/edit-race/edit-race.component';
import { RaceFormComponent } from './components/races/race-form/race-form.component';
import { PositionListComponent } from './components/races/position-list/position-list.component';
import { PositionFormComponent } from './components/races/position-form/position-form.component';
import { WeightListComponent } from './components/weights/weight-list/weight-list.component';
import { AddWeightComponent } from './components/weights/add-weight/add-weight.component';
import { EditWeightComponent } from './components/weights/edit-weight/edit-weight.component';
import { WeightFormComponent } from './components/weights/weight-form/weight-form.component';
import { ResearchSireViewportComponent } from './components/research/sire-viewport/sire-viewport.component';
import { ResearchWildcardViewportComponent } from './components/research/wildcard-viewport/wildcard-viewport.component';
import { ResearchFemaleViewportComponent } from './components/research/female-viewport/female-viewport.component';
import { ResearchAdminViewportComponent } from './components/research/admin-viewport/admin-viewport.component';
import { SalesAdmin } from './components/research/sales-admin/sales-admin.component';
import { SaleDialog } from './components/research/sales-admin/sales-admin.component';
import { BulkConfirm } from './components/research/sales-admin/sales-admin.component';
import { SalesAdminBulkAddComponent } from './components/salesreports/bulk-add/bulk-add.component';
import { InbreedingListComponent } from './components/research/inbreeding-list/inbreeding-list.component';
import { CommonAncestorsComponent } from './components/research/common-ancestors/common-ancestors.component';
import { TenGenerationsComponent } from './components/research/ten-generations/ten-generations.component';
import { TwinViewComponent } from './components/research/twin-view/twin-view.component';
import { DNAViewComponent } from './components/mt-dna/dna-view/dna-view.component';
import { FounderListComponent } from './components/mt-dna/founder-list/founder-list.component';
import { MtDNAAdminComponent } from './components/mt-dna/mtdna-admin/mtdna-admin.component';
import { MtDNAPopulationComponent } from './components/mt-dna/mtdna-population/mtdna-population.component';
import { MtDNAStallionComponent } from './components/mt-dna/mtdna-stallion/mtdna-stallion.component';
import { MtDNADistanceComponent } from './components/mt-dna/mtdna-distance/mtdna-distance.component';
import { AddHaploTypeDialog } from './components/mt-dna/mtdna-admin/add-haplotype-dialog';
import { StallionRatingsViewComponent} from './components/stallion-ratings/stallion-ratings-view/stallion-ratings-view.component';
import { RatingsListComponent } from './components/stallion-ratings/ratings-list/ratings-list.component';
import { StakesRecordComponent } from './components/races/stakes-record/stakes-record.component';
import { AncestryViewportComponent } from './components/ancestry/viewport/viewport.component';
import { AncestryAncestorValuesComponent } from './components/ancestry/ancestor-values/ancestor-values.component';
import { AncestryPopulationValuesComponent } from './components/ancestry/population-values/population-values.component';
import { FemaleLineSearchComponent } from './components/research/female-line-search/female-line-search.component';
import { FamilyStakesSearchComponent } from './components/research/family-stakes-search/family-stakes-search.component';
import { SireSearchComponent } from './components/research/sire-search/sire-search.component';
import { SireBroodmareSearchComponent } from './components/research/sire-broodmare-search/sire-broodmare-search.component';
import { SirelineSearchComponent } from './components/research/sireline-search/sireline-search.component';
import { SireBroodmareCrossesComponent } from './components/research/sire-broodmare-crosses/sire-broodmare-crosses.component';
import { SireBroodmareCrossesModal } from './components/research/sire-broodmare-crosses/sire-broodmare-crosses.modal.component';
import { WildcardSearchComponent } from './components/research/wildcard-search/wildcard-search.component';
import { QueryPositionComponent } from './components/research/query-position/query-position.component';
import { GrandparentsComponent } from './components/research/grandparents/grandparents.component';
import { MtDNALookupComponent } from './components/research/mtdna-lookup/mtdna-lookup.component';
import { StakesZCurrentChartComponent } from './components/stakes/zcurrent-chart/zcurrent-chart.component';
import { StakesZHistoricalChartComponent } from './components/stakes/zhistorical-chart/zhistorical-chart.component';
import { StakesPopulationViewportComponent } from './components/stakes/population-viewport/population-viewport.component';
import { MLModelBuilderComponent } from './components/machine-learning/model-builder/model-builder.component';
import { MtDNAFlagComponent } from './components/mt-dna/mtdna-flag/mtdna-flag.component';
import { SalesReportsComponent } from './components/salesreports/sales-reports.component';
import { ActiveSalesComponent } from './components/salesreports/activesales/active-sales.component';
import { SummaryReportComponent } from './components/salesreports/summary-report/summary-report.component';
import { BestReportComponent } from './components/salesreports/best-report/best-report-admin/best-report-admin.component';
import { MlRatingTableComponent } from './components/salesreports/best-report/ml-rating-table/ml-rating-table.component';
import { mtDNAHapTableComponent } from './components/salesreports/best-report/mtDNAHap-table/mtDNAHap-table.component';
import { SireTableComponent } from './components/salesreports/best-report/sire-table/sire-table.component';
import { OrderReportComponent } from './components/salesreports/order-report-dialog/order-report-dialog.component';
import { HaploTypeTableComponent } from './components/salesreports/order-report-dialog/halpo-type-table/haplo-type-table.component';
import { HaploGroupTableComponent } from './components/salesreports/order-report-dialog/haplo-group-table/haplo-group-table.component';
import { HaploOtherTableComponent } from './components/salesreports/order-report-dialog/haplo-other-table/haplo-other-table.component';

@NgModule({
  declarations: [
    AppComponent,
    SideNavbarComponent,
    TopNavbarComponent,
    MenuListItemComponent,
    HorseListComponent,
    HorseFormComponent,
    HorseFormExComponent,
    AddHorseComponent,
    AddHorsesComponent,
    EditHorseComponent,
    ConfirmComponent,
    AlertComponent,
    NotificationSnackComponent,
    HorseSearchComponent,
    HeirarchyTreeComponent,
    LineageTreeComponent,
    HierarchyViewComponent,
    LineageGraphComponent,
    LineageGraphDialogComponent,
    RadialLineageGraphComponent,
    LinebreedingListComponent,
    EquivalentsListComponent,
    HypotheticalHierarchyViewComponent,
    HorseListViewComponent,
    RaceListComponent,
    AddRaceComponent,
    EditRaceComponent,
    RaceFormComponent,
    PositionListComponent,
    PositionFormComponent,
    WeightListComponent,
    AddWeightComponent,
    EditWeightComponent,
    WeightFormComponent,
    ResearchSireViewportComponent,
    ResearchWildcardViewportComponent,
    ResearchFemaleViewportComponent,
    ResearchAdminViewportComponent,
    SalesAdmin,
    SaleDialog,
    BulkConfirm,
    SalesAdminBulkAddComponent,
    InbreedingListComponent,
    CommonAncestorsComponent,
    TenGenerationsComponent,
    TwinViewComponent,
    DNAViewComponent,
    FounderListComponent,
    MtDNAAdminComponent,
    MtDNAPopulationComponent,
    MtDNAStallionComponent,
    MtDNADistanceComponent,
    AddHaploTypeDialog,
    RatingsListComponent,
    StallionRatingsViewComponent,
    StakesRecordComponent,
    AncestryViewportComponent,
    AncestryAncestorValuesComponent,
    AncestryPopulationValuesComponent,
    FemaleLineSearchComponent,
    FamilyStakesSearchComponent,
    SireSearchComponent,
    SireBroodmareSearchComponent,
    SirelineSearchComponent,
    SireBroodmareCrossesComponent,
    WildcardSearchComponent,
    QueryPositionComponent,
    GrandparentsComponent,
    StakesZCurrentChartComponent,
    StakesZHistoricalChartComponent,
    StakesPopulationViewportComponent,
    SireBroodmareCrossesModal,
    MLModelBuilderComponent,
    MtDNAFlagComponent,
    MtDNALookupComponent,
    SalesReportsComponent,
    ActiveSalesComponent,
    SummaryReportComponent,
    BestReportComponent,
    MlRatingTableComponent,
    mtDNAHapTableComponent,
    SireTableComponent,
    OrderReportComponent,
    HaploTypeTableComponent,
    HaploGroupTableComponent,
    HaploOtherTableComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    
    RouterModule.forRoot([
      { path: '', redirectTo: 'horses', pathMatch: 'full' },
      { path: 'horses', component: HorseListComponent},
      { path: 'horses/:horseId', component: EditHorseComponent },
      { path: 'horses/add/new', component: AddHorseComponent },
      { path: 'horses/add/bulk', component: AddHorsesComponent },
      { path: 'charts/heirarchy/:horseId', component: HeirarchyTreeComponent },
      { path: 'charts/ancestry/:horseId', component: HierarchyViewComponent },
      { path: 'charts/hypothetical/mating', component: HypotheticalHierarchyViewComponent },
      { path: 'races', component: RaceListComponent},
      { path: 'races/add', component: AddRaceComponent},
      { path: 'races/:raceId', component: EditRaceComponent },
      { path: 'positions', component: PositionListComponent },
      { path: 'weights', component: WeightListComponent},
      { path: 'weights/add', component: AddWeightComponent},
      { path: 'weights/:weightId', component: EditWeightComponent },
      { path: 'research/sire/:tab', component: ResearchSireViewportComponent },
      { path: 'research/wildcard/:tab', component: ResearchWildcardViewportComponent },
      { path: 'research/female/:tab', component: ResearchFemaleViewportComponent },
      { path: 'research/admin/:tab', component: ResearchAdminViewportComponent },
      { path: 'research/admin/sales_admin/add/bulk/:auctionId', component: SalesAdminBulkAddComponent },
      { path: 'mtdna/:tab', component: DNAViewComponent },
      { path: 'stallion-ratings/:tab', component: StallionRatingsViewComponent},
      { path: 'ancestry/:tab', component: AncestryViewportComponent },
      { path: 'stakes/population/:tab', component: StakesPopulationViewportComponent },
      { path: 'ml', component: MLModelBuilderComponent},
      { path: 'sales_reports/:tab', component: SalesReportsComponent},

    ]),
    BrowserAnimationsModule,
    FlexLayoutModule,
    AngularMaterialModule,    
    SatPopoverModule,
    MatStepperModule,
  ],
  entryComponents: [ConfirmComponent, AlertComponent, NotificationSnackComponent, HorseSearchComponent, AddHaploTypeDialog, SireBroodmareCrossesModal, HorseFormExComponent, SaleDialog, BulkConfirm, SummaryReportComponent, BestReportComponent, OrderReportComponent],
  providers: [NavService, ChartService, HorseService, RaceService, StyleUtils, { provide: Window, useValue: window }],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})

export class AppModule { }
