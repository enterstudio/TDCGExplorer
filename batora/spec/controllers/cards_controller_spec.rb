require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe CardsController do

  def mock_player(stubs={})
    @mock_player ||= mock_model(Player, stubs)
  end

  def player_cards
    @player_cards ||= mock("player cards")
  end

  def mock_card(stubs={})
    @mock_card ||= mock_model(Card, stubs)
  end

  before do
    Player.stub!(:find).and_return(mock_player(:cards => player_cards))
  end

  def mock_user(stubs={})
    @mock_user ||= mock_model(User, stubs)
  end

  before do
    controller.stub!(:current_user).and_return(mock_user)
  end

  describe "GET index" do
    it "assigns all cards as @cards" do
      player_cards.stub!(:find).with(:all, :include => :character).and_return([mock_card])
      get :index
      assigns[:cards].should == [mock_card]
    end
  end

  describe "GET show" do
    it "assigns the requested card as @card" do
      player_cards.stub!(:find).with("37").and_return(mock_card)
      get :show, :id => "37"
      assigns[:card].should equal(mock_card)
    end
  end

  describe "GET new" do
    it "assigns a new card as @card" do
      player_cards.stub!(:build).and_return(mock_card)
      get :new
      assigns[:card].should equal(mock_card)
    end
  end

  describe "GET edit" do
    it "assigns the requested card as @card" do
      player_cards.stub!(:find).with("37").and_return(mock_card)
      get :edit, :id => "37"
      assigns[:card].should equal(mock_card)
    end
  end

  describe "POST create" do

    describe "with valid params" do
      it "assigns a newly created card as @card" do
        player_cards.stub!(:build).with({'these' => 'params'}).and_return(mock_card(:save => true))
        post :create, :card => {:these => 'params'}
        assigns[:card].should equal(mock_card)
      end

      it "redirects to the created card" do
        player_cards.stub!(:build).and_return(mock_card(:save => true))
        post :create, :card => {}
        response.should redirect_to(player_card_url(mock_player, mock_card))
      end
    end

    describe "with invalid params" do
      it "assigns a newly created but unsaved card as @card" do
        player_cards.stub!(:build).with({'these' => 'params'}).and_return(mock_card(:save => false))
        post :create, :card => {:these => 'params'}
        assigns[:card].should equal(mock_card)
      end

      it "re-renders the 'new' template" do
        player_cards.stub!(:build).and_return(mock_card(:save => false))
        post :create, :card => {}
        response.should render_template('new')
      end
    end

  end

  describe "PUT update" do

    describe "with valid params" do
      it "updates the requested card" do
        player_cards.should_receive(:find).with("37").and_return(mock_card)
        mock_card.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :card => {:these => 'params'}
      end

      it "assigns the requested card as @card" do
        player_cards.stub!(:find).and_return(mock_card(:update_attributes => true))
        put :update, :id => "1"
        assigns[:card].should equal(mock_card)
      end

      it "redirects to the card" do
        player_cards.stub!(:find).and_return(mock_card(:update_attributes => true))
        put :update, :id => "1"
        response.should redirect_to(player_card_url(mock_player, mock_card))
      end
    end

    describe "with invalid params" do
      it "updates the requested card" do
        player_cards.should_receive(:find).with("37").and_return(mock_card)
        mock_card.should_receive(:update_attributes).with({'these' => 'params'})
        put :update, :id => "37", :card => {:these => 'params'}
      end

      it "assigns the card as @card" do
        player_cards.stub!(:find).and_return(mock_card(:update_attributes => false))
        put :update, :id => "1"
        assigns[:card].should equal(mock_card)
      end

      it "re-renders the 'edit' template" do
        player_cards.stub!(:find).and_return(mock_card(:update_attributes => false))
        put :update, :id => "1"
        response.should render_template('edit')
      end
    end

  end

  describe "DELETE destroy" do
    it "destroys the requested card" do
      player_cards.should_receive(:find).with("37").and_return(mock_card)
      mock_card.should_receive(:destroy)
      delete :destroy, :id => "37"
    end

    it "redirects to the cards list" do
      player_cards.stub!(:find).and_return(mock_card(:destroy => true))
      delete :destroy, :id => "1"
      response.should redirect_to(player_cards_url(mock_player))
    end
  end

end
