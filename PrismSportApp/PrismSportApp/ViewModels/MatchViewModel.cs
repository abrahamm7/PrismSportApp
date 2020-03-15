﻿using Prism.Navigation;
using Prism.Services;
using PrismSportApp.Models;
using Refit;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Windows.Input;
using Xamarin.Forms;

namespace PrismSportApp.ViewModels
{
    public class MatchViewModel: INotifyPropertyChanged
    {
        #region Class
        public event PropertyChangedEventHandler PropertyChanged;
        public List<League> CompetitionsLists { get; set; } = new List<League>();
        public List<Match> Matches { get; set; } = new List<Match>();
        Fixtures Fixture { get; set; } = new Fixtures();
        Competitions League { get; set; } = new Competitions();
        Competitions LeagueSelect { get; set; } = new Competitions();
        public Links Links { get; set; } = new Links();
        INavigationService navigation;

        IPageDialogService dialogService;
        
        IApiServices apiServices;
        #endregion

        #region Commands and Properties
        public ICommand Selected { get; set; }
        #endregion

        #region Constructor
        public MatchViewModel(IApiServices api, INavigationService navigationService, IPageDialogService pageDialog)
        {
            navigation = navigationService;
            dialogService = pageDialog;
            apiServices = api;

            Selected = new Command(async(object sender) => 
            {
                LeagueSelect = (Competitions)sender;
            });
             
           
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Messages();
            }
            else
            {
                GetMatches();
            }
            
        }
        #endregion
        
       

        async Task GetMatches()
        {
            try
            {                
                RestService.For<IApiServices>(Links.url);

                var response1 = await apiServices.GetLeagues();

                var worldcup = await apiServices.GetFixturesWorldCup();

                var uefa = await apiServices.GetFixturesUefaChampions();

                League = response1;

                var show = League.competitions.Where(elemento => elemento.Id == 2000 || elemento.Id == 2001).ToList();

                Fixture = worldcup;
                              
                //Fixture = uefa;
                
                this.Matches = Fixture.Matches.Distinct().ToList();                

                this.CompetitionsLists = show; 
                
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Error en el metodo Leagues: {ex.Message}");
            }

        }

        void Messages()
        {
            dialogService.DisplayAlertAsync("Error", "Check your connection to internet", "ok");
        }

        
    }
}

