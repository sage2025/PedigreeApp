import { Component, AfterViewInit, Input } from '@angular/core';
import { MatSelectChange, MatTableDataSource } from '@angular/material';
import { MatDialog } from '@angular/material/dialog';

import { Subscription } from 'rxjs';
import { HaploGroup } from 'src/app/_models/haplo-group.model';
import { HaploType } from 'src/app/_models/haplo-type.model';
import { MtDnaService } from '../../../_services/mtdna.service';
import { GROUP_COLORS } from 'src/app/constants';
import { AddHaploTypeDialog } from './add-haplotype-dialog';

@Component({
    selector: 'app-mtdna-admin',
    templateUrl: './mtdna-admin.component.html',
    styleUrls: ['./mtdna-admin.component.scss']
})

export class MtDNAAdminComponent implements AfterViewInit {
    @Input() haploGroups: HaploGroup[];
    @Input() loadingData: boolean = false;


    private subscription: Subscription;
    private haploGroupsData: HaploGroup[] = [];
    haploTypeColumns: string[] = ['no', 'name', 'population', 'action'];
    haploTypeDataSources: object = {};


    readonly groupColors = GROUP_COLORS;

    constructor(
        private mtDnaService: MtDnaService,
        public haploTypeDialog: MatDialog
    ) {

    }

    ngAfterViewInit() {
        
    }

    ngOnChanges() {
        this.haploGroupsData = this.haploGroups.filter(g => g.title != 'UNK');
    }
    changeGroupColor(group: HaploGroup, $event: MatSelectChange) {
        const color = $event.value;

        this.mtDnaService!.updateHaploGroupColor(group.id, color)
            .subscribe(
                data => {
                    group.color = color;
                },
                error => {
                    console.error(error);
                });
    }

    openAddHaploType(group: HaploGroup): void {
        const dialogRef = this.haploTypeDialog.open(AddHaploTypeDialog, {
            width: '400px',
            autoFocus: true,
            data: { group: group },
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result.length == 0) return;
            this.mtDnaService!.addHaploType(group.id, result)
                .subscribe(
                    data => {
                        group.types.push({ id: parseInt(data.toString()), name: result, groupId: group.id });

                        this.typeDataSource(group).data = group.types;
                    },
                    error => {
                        console.error(error);
                    });
        });
    }

    removeHaploType(group: HaploGroup, type: HaploType): void {

        this.mtDnaService!.deleteHaploType(type.id)
            .subscribe(
                data => {
                    const index = group.types.indexOf(type);

                    if (index >= 0) {
                        group.types.splice(index, 1);
                        this.typeDataSource(group).data = group.types;
                    }
                },
                error => {
                    console.error(error);
                });
    }

    typeDataSource(group) {
        var dataSource = this.haploTypeDataSources[group.id];
        if (!dataSource) dataSource = new MatTableDataSource<HaploType>();

        dataSource.data = group.types;
        return dataSource;
    }
}
