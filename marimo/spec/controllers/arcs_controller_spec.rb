require File.expand_path(File.dirname(__FILE__) + '/../spec_helper')

describe ArcsController do
  before do
    controller.stub!(:current_user).and_return(mock_user)
  end

  def mock_user(stubs={})
    @_mock_user ||= mock_model(User, stubs)
  end

  def mock_arc(stubs={})
    @_mock_arc ||= mock_model(Arc, stubs)
  end

  describe "GET index" do

    it "arcs �𓾂�" do
      Arc.should_receive(:paginate).and_return([ mock_arc ])
      get :index
      assigns[:arcs].should == [ mock_arc ]
    end

  end

  describe "GET show" do

    it "�w�� arc �𓾂�" do
      Arc.should_receive(:find).with("42").and_return(mock_arc)
      get :show, :id => "42"
      assigns[:arc].should == mock_arc
    end

  end

  describe "GET edit" do

    it "�w�� arc �𓾂�" do
      Arc.should_receive(:find).with("42").and_return(mock_arc)
      get :edit, :id => "42"
      assigns[:arc].should == mock_arc
    end

    it "�v user �F��" do
      controller.should_receive(:current_user).and_return(mock_user)
      Arc.stub!(:find).and_return(mock_arc)
      get :edit, :id => "1"
    end

  end

  describe "PUT update" do

    it "�w�� arc ���X�V����" do
      Arc.should_receive(:find).with("42").and_return(mock_arc)
      mock_arc.should_receive(:update_attributes).with({'these' => 'params'}).and_return(true)
      put :update, :id => "42", :arc => {:these => 'params'}
      response.should redirect_to(arc_path(mock_arc))
    end

    it "�v user �F��" do
      controller.should_receive(:current_user).and_return(mock_user)
      Arc.stub!(:find).and_return(mock_arc)
      mock_arc.stub!(:update_attributes).and_return(true)
      put :update, :id => "1"
    end

  end
end